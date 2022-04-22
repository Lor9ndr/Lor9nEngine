using Lor9nEngine.Rendering.Base;
using Lor9nEngine.Rendering.Interfaces;
using Lor9nEngine.Rendering.Textures;

using OpenTK.Mathematics;

namespace Lor9nEngine
{
    internal static class ModelFactory
    {
        private static readonly Vertex[] _cubeVertices = {
        // positions
        new Vertex(new Vector3(-1.0f,  1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
        new Vertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3(1.0f,  1.0f, -1.0f), new Vector2(0.0f, 1.0f)),
        new Vertex(new Vector3(-1.0f,  1.0f, -1.0f),new Vector2(0.0f, 0.0f)),

        new Vertex(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(0.0f, 0.0f)),
        new Vertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3(-1.0f,  1.0f, -1.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3(-1.0f,  1.0f, -1.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3(-1.0f,  1.0f,  1.0f), new Vector2(0.0f, 1.0f)),
        new Vertex(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(0.0f, 0.0f)),

        new Vertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3( 1.0f, -1.0f,  1.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3( 1.0f,  1.0f,  1.0f), new Vector2(0.0f, 1.0f)),
        new Vertex(new Vector3( 1.0f,  1.0f,  1.0f), new Vector2(0.0f, 1.0f)),
        new Vertex(new Vector3( 1.0f,  1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
        new Vertex(new Vector3( 1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),

        new Vertex(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3(-1.0f,  1.0f,  1.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3( 1.0f,  1.0f,  1.0f), new Vector2(0.0f, 1.0f)),
        new Vertex(new Vector3( 1.0f,  1.0f,  1.0f), new Vector2(0.0f, 1.0f)),
        new Vertex(new Vector3( 1.0f, -1.0f,  1.0f), new Vector2(0.0f, 0.0f)),
        new Vertex(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(1.0f, 0.0f)),

        new Vertex(new Vector3(-1.0f,  1.0f, -1.0f), new Vector2(0.0f, 1.0f)),
        new Vertex(new Vector3( 1.0f,  1.0f, -1.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3( 1.0f,  1.0f,  1.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3( 1.0f,  1.0f,  1.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3(-1.0f,  1.0f,  1.0f), new Vector2(0.0f, 0.0f)),
        new Vertex(new Vector3(-1.0f,  1.0f, -1.0f), new Vector2(0.0f, 1.0f)),

        new Vertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(0.0f, 1.0f)),
        new Vertex(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3( 1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3( 1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(0.0f, 0.0f)),
        new Vertex(new Vector3( 1.0f, -1.0f,  1.0f), new Vector2(0.0f, 1.0f))
    };
        private static readonly Mesh _mesh = new Mesh(_cubeVertices, "Cube");
        private static readonly ITexture _lightTexture = Texture.LoadFromFile(Game.TEXTURES_PATH + "/blub.png", TextureType.Diffuse, string.Empty);
        public static Model GetTexturedCube(ITexture texture)
        {
            var textures = new List<ITexture>() { texture };
            return new Model(new Mesh("cubeTextured", _mesh.ObjectSetupper, textures), "CubeTextured");
        }
        public static Model GetLightModel() => GetTexturedCube(_lightTexture);
        public static Model GetSphere = new(Game.OBJ_PATH + "Sphere.obj");
        public static Model GetCube() => new Model(_mesh, "Cube");
        public static Model GetDefaultCube = new Model(_mesh, Texture.GetDefaultTextures, "Cube");
        public static Model GetNanoSuitModel() => new(Game.NANOSUIT_PATH);
        public static Model GetBridgeModel() => new(Game.BRIDGE_PATH);
        public static Model GetManModel() => new(Game.MAN_PATH);
        public static Model GetTerrainModel() => new(Game.TERRAIN_PATH);
        //public static Model GetFloorModel(Vector3 position) => new(Game.FLOOR_PATH, new Transform(position, new Vector3(0), new Vector3(0), new Vector3(0.05f), new Vector3(0)));
        public static Model GetCostumeModel() => new(Game.OBJ_PATH + "PBR/tv.obj");
        public static Model GetM4A1() => new(Game.OBJ_PATH + "m4a1/scene.gltf");
        public static Model GetLotr() => new(Game.OBJ_PATH + "lotr_troll/scene.gltf");
        public static Model GetPlane() => new(Game.OBJ_PATH + "Plane.obj");
        public static Model GetIcoSphere() => new(Game.OBJ_PATH + "icosphere.obj");
        public static Model GetFrog() => new(Game.OBJ_PATH + "Animated/Frog/Basic Kermit.fbx");
        public static Model GetDancingVampire() => new(Game.OBJ_PATH + "Animated/vampire/dancing_vampire.dae");
        public static Model GetPBRTV() => new(Game.OBJ_PATH + "PBR/tv.obj");
        public static Model GetPBRRobot() => new(Game.OBJ_PATH + "PBR/Robot/Anvil.FBX");
        public static Model GetAtrium() => new(Game.OBJ_PATH + "SponzaAtrium/sponza.obj");
        public static Model GetWorkPlace() => new(Game.WORKPLACE_OBJ);
    }
}
