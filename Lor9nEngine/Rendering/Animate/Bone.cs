using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Lor9nEngine.Rendering.Animate
{
    public class Bone
    {
        private readonly List<KeyPosition> _positions = new List<KeyPosition>();
        private readonly List<KeyRotation> _rotations = new List<KeyRotation>();
        private readonly List<KeyScale> _scales = new List<KeyScale>();
        private readonly int _numPositions;
        private readonly int _numRotations;
        private readonly int _numScalings;

        private Matrix4 _localTransform;
        private readonly string _name;
        private readonly int _id;

        public Bone(string name, int id, Assimp.NodeAnimationChannel channel)
        {
            _name = name;
            _id = id;
            _localTransform = Matrix4.Identity;

            _numPositions = channel.PositionKeyCount;

            for (int positionIndex = 0; positionIndex < _numPositions; positionIndex++)
            {
                Assimp.Vector3D aiPosition = channel.PositionKeys[positionIndex].Value;
                double timeStamp = channel.PositionKeys[positionIndex].Time;
                KeyPosition data;
                data.Position = new Vector3(aiPosition.X, aiPosition.Y, aiPosition.Z);
                data.TimeStamp = Convert.ToSingle(timeStamp);
                _positions.Add(data);
            }
            _numRotations = channel.RotationKeyCount;
            for (int rotationIndex = 0; rotationIndex < _numRotations; rotationIndex++)
            {
                Assimp.Quaternion aiRotation = channel.RotationKeys[rotationIndex].Value;
                double timeStamp = channel.RotationKeys[rotationIndex].Time;
                KeyRotation data;
                data.Orientation = new Quaternion(aiRotation.X, aiRotation.Y, aiRotation.Z, aiRotation.W);
                data.TimeStamp = Convert.ToSingle(timeStamp);
                _rotations.Add(data);
            }
            _numScalings = channel.ScalingKeyCount;
            for (int scalingIndex = 0; scalingIndex < _numScalings; scalingIndex++)
            {
                Assimp.Vector3D aiScale = channel.ScalingKeys[scalingIndex].Value;
                double timeStamp = channel.ScalingKeys[scalingIndex].Time;
                KeyScale data;
                data.Scale = new Vector3(aiScale.X, aiScale.Y, aiScale.Z);
                data.TimeStamp = Convert.ToSingle(timeStamp);
                _scales.Add(data);
            }
        }
        public void Update(float animationTime)
        {
            Matrix4 translation = InterpolatePosition(animationTime);
            Matrix4 rotation = InterpolateRotation(animationTime);
            Matrix4 scale = InterpolateScaling(animationTime);
            _localTransform = rotation * scale * translation;
        }

        public Matrix4 GetLocalTransform() => _localTransform;
        public string GetBoneName() => _name;
        public int GetBoneID => _id;
        public int GetPositionIndex(float animationTime)
        {
            for (int index = 0; index < _numPositions - 1; ++index)
            {
                if (animationTime < _positions[index + 1].TimeStamp)
                    return index;
            }
            return 0;
        }
        public int GetRotationIndex(float animationTime)
        {
            for (int index = 0; index < _numRotations - 1; ++index)
            {
                if (animationTime < _rotations[index + 1].TimeStamp)
                    return index;
            }
            return 0;
        }

        public int GetScaleIndex(float animationTime)
        {
            for (int index = 0; index < _numScalings - 1; ++index)
            {
                if (animationTime < _scales[index + 1].TimeStamp)
                    return index;
            }
            return 0;
        }

        private float GetScaleFactor(float lastTimeStamp, float nextTimeStamp, float animationTime)
        {
            float midWayLength = animationTime - lastTimeStamp;
            float framesDiff = nextTimeStamp - lastTimeStamp;
            return midWayLength / framesDiff;
        }
        private Matrix4 InterpolatePosition(float animationTime)
        {
            if (1 == _numPositions)
            {
                return Matrix4.CreateTranslation(_positions[0].Position);
            }
            int p0Index = GetPositionIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(_positions[p0Index].TimeStamp, _positions[p1Index].TimeStamp, animationTime);
            Vector3 finalPosition = Vector3.Lerp(_positions[p0Index].Position, _positions[p1Index].Position, scaleFactor);
            return Matrix4.CreateTranslation(finalPosition);
        }
        private Matrix4 InterpolateRotation(float animationTime)
        {
            if (1 == _numRotations)
            {
                return Matrix4.CreateFromQuaternion(Quaternion.Normalize(_rotations[0].Orientation));

            }
            int p0Index = GetRotationIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(_rotations[p0Index].TimeStamp, _rotations[p1Index].TimeStamp, animationTime);
            Quaternion finalPosition = Quaternion.Slerp(_rotations[p0Index].Orientation, _rotations[p1Index].Orientation, scaleFactor);
            return Matrix4.CreateFromQuaternion(finalPosition);
        }
        private Matrix4 InterpolateScaling(float animationTime)
        {
            if (1 == _numScalings)
            {
                return Matrix4.CreateTranslation(_scales[0].Scale);
            }
            int p0Index = GetScaleIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(_scales[p0Index].TimeStamp, _scales[p1Index].TimeStamp, animationTime);
            Vector3 finalPosition = Vector3.Lerp(_scales[p0Index].Scale, _scales[p1Index].Scale, scaleFactor);
            return Matrix4.CreateScale(finalPosition);
        }


    }
}
