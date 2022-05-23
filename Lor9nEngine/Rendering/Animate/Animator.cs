using Lor9nEngine.Rendering.Interfaces;

using Newtonsoft.Json;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace Lor9nEngine.Rendering.Animate
{
    public class Animator : IRenderable, IUpdatable
    {
        private readonly List<Matrix4> _finalBoneMatrices;
        private Animation? _currentAnimation;
        private float _currentTime;
        public bool HasAnimation => _currentAnimation is not null;
        [JsonConstructor]
        internal Animator(Animation? animation)
        {
            _currentTime = 0.0f;
            _currentAnimation = animation;
            _finalBoneMatrices = new List<Matrix4>();
            for (int i = 0; i < 100; i++)
            {
                _finalBoneMatrices.Add(Matrix4.Identity);
            }
        }

        public async Task UpdateAsync()
        {
            if (HasAnimation)
            {
                await CalculateBoneTransform(_currentAnimation.GetRootNode(), Matrix4.Identity);
            }
        }
        public void Update()
        {
            if (HasAnimation)
            {
                _currentTime += _currentAnimation.GetTicksPerSecond() * Game.DeltaTime;
                _currentTime = _currentTime % _currentAnimation.GetDuration();
            }
        }
        public void PlayAnimation(Animation animation)
        {
            _currentAnimation = animation;
            _currentTime = 0.0f;
        }

        internal async Task CalculateBoneTransform(AssimpNodeData node, Matrix4 parentTranform)
        {
            string nodeName = node.Name;
            if (nodeName is null)
            {
                return;
            }
            Matrix4 nodeTransform = node.Transformation;
            Bone bone = _currentAnimation.FindBone(nodeName);
            if (bone is not null)
            {
                bone.Update(_currentTime);
                nodeTransform = bone.GetLocalTransform();
            }

            Matrix4 globalTransformation = nodeTransform * parentTranform;
            var boneInfoMap = _currentAnimation.GetBoneInfoMap();
            if (boneInfoMap.TryGetValue(nodeName, out BoneInfo boneInfo))
            {
                int index = boneInfo.ID;
                Matrix4 offset = boneInfo.offset;
                _finalBoneMatrices[index] = offset * globalTransformation;
            }
            for (int i = 0; i < node.ChildrenCount; i++)
            {
                await CalculateBoneTransform(node.children[i], globalTransformation);
            }
        }
        internal List<Matrix4> GetFinalBoneMatrices() => _finalBoneMatrices;

        public void Render(Shader shader)
        {
            EngineGL.Instance.UseShader(shader);
            Render();
        }
        public void Render()
        {
            for (int i = 0; i < _finalBoneMatrices.Count; i++)
            {
                EngineGL.Instance.SetShaderData($"finalBonesMatrices[{i}]", _finalBoneMatrices[i]);
            }
        }

        public void Render(PrimitiveType type = PrimitiveType.Triangles)
        {
            Render();
        }

        public void Dispose()
        {
        }

        void IRenderable.RenderWithOutTextures(Shader shader)
        {
            throw new NotImplementedException();
        }
    }
}
