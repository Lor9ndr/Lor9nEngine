using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lor9nEngine.Rendering.Base
{
    public struct Vertex
    {
        public const int MAX_BONE_INFLUENCE = 4;
        public static int Size => ((3 + 3 + 2 + 3 + 3 + 4) * sizeof(float)) + 4 * sizeof(int);
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoords;
        public Vector3 Tangent;
        public Vector3 Bitangent;
        public Vector4i BoneIDs;
        public Vector4 Weights;
        public Vertex(Vector3 position)
        {
            Position = position;
            Normal = new Vector3();
            TexCoords = new Vector2();
            Tangent = new Vector3();
            Bitangent = new Vector3();
            BoneIDs = new Vector4i(-1);
            Weights = new Vector4(0);
        }
        public Vertex(float x, float y, float z) : this(new Vector3(x, y, z)) { }

        public Vertex(Vector3 position, Vector2 texCoords)
            : this(position) => TexCoords = texCoords;
        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoords)
            : this(position, texCoords) => Normal = normal;
        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoords, Vector3 tangent, Vector3 bitangent)
            : this(position, normal, texCoords)
        {
            Tangent = tangent;
            Bitangent = bitangent;
        }

        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoords, Vector3 tangent, Vector3 bitangent, Vector4i boneIDs)
            : this(position, normal, texCoords, tangent, bitangent) => BoneIDs = boneIDs;

        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoords, Vector3 tangent, Vector3 bitangent, Vector4i boneIDs, Vector4 weights)
            : this(position, normal, texCoords, tangent, bitangent, boneIDs) => Weights = weights;
    }
}
