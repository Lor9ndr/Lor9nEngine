using Assimp;

using Lor9nEngine.Extensions;
using Lor9nEngine.Rendering.Base;

using Newtonsoft.Json;

using OpenTK.Mathematics;

namespace Lor9nEngine.Rendering.Animate
{
    internal class Animation
    {
        private readonly float _duration;
        private readonly float _ticksPerSecond;
        private readonly List<Bone> _bones = new List<Bone>();
        private AssimpNodeData _rootNode;
        private Dictionary<string, BoneInfo> _boneInfoMap;

        [JsonConstructor]
        internal Animation(string animationPath, Model model, int index = 0)
        {
            using AssimpContext importer = new();
            if (!importer.IsImportFormatSupported(Path.GetExtension(animationPath)))
            {
                throw new ArgumentException("Model format " + Path.GetExtension(animationPath) + " is not supported!  Cannot load {1}", "filename");
            }
            Scene scene = importer.ImportFile(animationPath);
            var animation = scene.Animations[index];
            _duration = Convert.ToSingle(animation.DurationInTicks);
            _rootNode.Name = Guid.NewGuid().ToString();
            _ticksPerSecond = Convert.ToSingle(animation.TicksPerSecond);
            ReadHierarchyData(ref _rootNode, scene.RootNode);
            ReadMissingBones(animation, model);
        }

        public Bone? FindBone(string name) => _bones.FirstOrDefault(s => s.GetBoneName() == name);
        public void AddBone(Bone bone) => _bones.Add(bone);
        public float GetTicksPerSecond() => _ticksPerSecond;
        public float GetDuration() => _duration;
        public AssimpNodeData GetRootNode() => _rootNode;
        public Dictionary<string, BoneInfo> GetBoneInfoMap() => _boneInfoMap;

        private void ReadMissingBones(Assimp.Animation animation, Model model)
        {
            int size = animation.NodeAnimationChannelCount;
            var boneInfoMap = model.GetBoneInfoMap();
            int boneCount = model.GetBoneCount();

            for (int i = 0; i < size; i++)
            {
                var channel = animation.NodeAnimationChannels[i];
                string boneName = channel.NodeName;

                if (!boneInfoMap.ContainsKey(boneName))
                {
                    boneInfoMap.Add(boneName, new BoneInfo() { ID = boneCount, offset = Matrix4.Identity });
                    boneCount++;
                }
                _bones.Add(new Bone(boneName, boneInfoMap[boneName].ID, channel));
            }

            _boneInfoMap = boneInfoMap;
        }

        private void ReadHierarchyData(ref AssimpNodeData dest, Node src)
        {
            dest.Name = src.Name;
            if (dest.Name is null)
            {
                dest.Name = Guid.NewGuid().ToString();
            }
            dest.Transformation = src.Transform.GetMatrix4FromAssimpMatrix();
            dest.ChildrenCount = src.ChildCount;
            if (dest.children is null)
            {
                dest.children = new List<AssimpNodeData>();
            }
            for (int i = 0; i < src.ChildCount; i++)
            {
                AssimpNodeData newData = new AssimpNodeData();
                ReadHierarchyData(ref newData, src.Children[i]);
                dest.children.Add(newData);
            }
        }
    }
}
