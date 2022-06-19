using Assimp;

using Lor9nEngine.Extensions;
using Lor9nEngine.Rendering.Animate;
using Lor9nEngine.Rendering.Interfaces;
using Lor9nEngine.Rendering.Textures;

using NLog;

using OpenTK.Mathematics;

namespace Lor9nEngine.Rendering.Base
{

    public class Model : IRenderable, IUpdatable
    {

        #region Private Properties
        private readonly string directory;
        private readonly string _path;
        /// <summary>
        /// Словарь уже существующих текстур, где ключ - путь, значение - модель
        /// </summary>
        private static readonly Dictionary<string, Model> _models = new();

        /// <summary>
        /// Список загруженных текстур
        /// </summary>
        private static readonly List<ITexture> _loadedTextures = new();

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Properties

        /// <summary>
        /// Список мешей
        /// </summary>
        public List<Mesh> Meshes = new();

        /// <summary>
        /// Словарь костей, для анимаций
        /// </summary>
        private readonly Dictionary<string, BoneInfo> _boneInfoMap = new Dictionary<string, BoneInfo>();

        /// <summary>
        /// Кол-во костей
        /// </summary>
        private int _boneCounter = 0;

        /// <summary>
        /// Максимальный вес костей
        /// </summary>
        public const int MAX_BONE_WEIGHTS = 100;

        /// <summary>
        /// Геттер получения словаря костей
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, BoneInfo> GetBoneInfoMap() => _boneInfoMap;

        /// <summary>
        /// Геттер получения кол-во костей
        /// </summary>
        /// <returns></returns>
        public int GetBoneCount() => _boneCounter;

        /// <summary>
        /// Класс отвечающий за воспроизведение анимаций
        /// </summary>
        public Animator Animator;

        /// <summary>
        /// Список анимаций, 
        /// TODO: стоит возможно сделать статичным и загружать туда все анимации и воспроизводить из <see cref="Animator"/>
        /// </summary>
        public Dictionary<string, Animate.Animation> Animations;

        #endregion

        #region Constructor
        /// <summary>
        /// Конструктор модели по пути
        /// </summary>
        /// <param name="path">Путь, где находится модель</param>
        public Model(string path)
        {
            directory = path[..path.LastIndexOf('/')];
            _path = path;
            // Если модель не существует в словаре моделей, то загружаем её
            if (!CheckModels(_path))
            {
                LoadModel();
                //GetOrSetThread();
            }
            if (Animator is null)
            {
                Animator = new Animator(null);

            }
        }

        /// <summary>
        /// Создание модели из 1 меша
        /// </summary>
        /// <param name="mesh">Сам меш</param>
        /// <param name="name">Название модели, которое пойдет как путь модели</param>
        public Model(Mesh mesh, string name)
        {
            if (!CheckModels(name))
            {
                Meshes.Add(mesh);
                Animator = new Animator(null);
                _path = name;
            }
        }

        /// <summary>
        /// Конструктор модели из 1 меша и списка текстур по имени
        /// </summary>
        /// <param name="mesh">Сам меш</param>
        /// <param name="textures">Список текстур</param>
        /// <param name="name">Наименование модели, пойдет как путь модели</param>
        public Model(Mesh mesh, List<ITexture> textures, string name)
            : this(mesh, name)
        {
            if (!CheckModels(name))
            {
                Meshes.Add(mesh);
                Animator = new Animator(null);
                _path = name;
                mesh.Textures = textures;
            }
        }
        public Model(List<Mesh> meshes, string name, string path)
        {
            _path = path;
            if (!CheckModels(name))
            {
                Meshes.AddRange(meshes);
                _path = name;
            }
            else
            {
                LoadModel();
            }
        }

        public void RenderWithOutTextures(Shader shader)
        {
            if (Animator.HasAnimation)
            {
                Animator?.Render();
            }

            for (int i = 0; i < Meshes.Count; i++)
            {
                Meshes[i].RenderWithOutTextures(shader);
            }
        }

        public void Render(Shader shader)
        {
            if (Animator.HasAnimation)
            {
                Animator?.Render();
            }

            for (int i = 0; i < Meshes.Count; i++)
            {
                Meshes[i].Render(shader);
            }
        }

        public void Update()
        {
            if (Animator.HasAnimation)
            {
                Animator.Update();
            }
        }

        public async Task UpdateAsync()
        {
            if (Animator.HasAnimation)
            {
                await Animator.UpdateAsync();
            }
        }

        #region ASSIMP METHODS
        private void ProcessNode(Node node, Scene scene)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                Mesh m = ProcessMesh(mesh, scene);
                Meshes.Add(m);

            }
            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }
        private bool CheckModels(string path)
        {
            if (path == null)
            {
                return false;
            }
            if (_models.TryGetValue(path, out var m))
            {
                Meshes.AddRange(m.Meshes);
                return true;
            }
            return false;
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            List<Vertex> vertices = new();
            List<int> indices = new();
            List<ITexture> textures = new();
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex v = new(new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z));
                if (mesh.HasTextureCoords(0))
                    v.TexCoords = new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y);

                if (mesh.HasTangentBasis)
                {
                    v.Tangent = new Vector3(mesh.Tangents[i].X, mesh.Tangents[i].Y, mesh.Tangents[i].Z);
                    v.Bitangent = new Vector3(mesh.BiTangents[i].X, mesh.BiTangents[i].Y, mesh.BiTangents[i].Z);
                }

                if (mesh.HasNormals)
                    v.Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);

                vertices.Add(v);
            }

            indices.AddRange(mesh.GetIndices().Select(s => Convert.ToInt32(s)));

            Material material = scene.Materials[mesh.MaterialIndex];
            textures.AddRange(LoadMaterialTexture(material, Assimp.TextureType.Diffuse, Textures.TextureType.Diffuse));

            textures.AddRange(LoadMaterialTexture(material, Assimp.TextureType.Specular, Textures.TextureType.Specular));

            textures.AddRange(LoadMaterialTexture(material, Assimp.TextureType.Normals, Textures.TextureType.Normal));

            textures.AddRange(LoadMaterialTexture(material, Assimp.TextureType.Height, Textures.TextureType.Roughness));

            textures.AddRange(LoadMaterialTexture(material, Assimp.TextureType.Opacity, Textures.TextureType.Alpha));


            if (textures.Count == 0)
            {
                textures.AddRange(Texture2D.GetDefaultTextures);
            }
            if (mesh.HasBones)
            {
                ExtractBoneWeightForVertices(vertices, mesh);
            }

            return new Mesh(vertices.ToArray(), mesh.Name, textures, indices.ToArray());

        }
        private List<ITexture> LoadMaterialTexture(Material mat, Assimp.TextureType type, Textures.TextureType typeName)
        {
            List<ITexture> textures = new();

            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                mat.GetMaterialTexture(type, i, out TextureSlot str);
                bool skip = false;
                for (int j = 0; j < _loadedTextures.Count; j++)
                {
                    if (_loadedTextures[j].Path == str.FilePath)
                    {
                        textures.Add(_loadedTextures[j]);
                        skip = true;
                        break;
                    }
                }
                if (!skip)
                {
                    if (str.FilePath != null)
                    {
                        ITexture texture = Texture2D.LoadFromFile(str.FilePath, typeName, directory);
                        textures.Add(texture);
                        _loadedTextures.Add(texture);
                    }

                }
            }

            return textures;
        }
        private void LoadModel()
        {
            _logger.Info($"Loading file: {_path}");
            if (_models.TryGetValue(_path, out var m))
            {
                _logger.Info($"Find in previously loaded files");

                foreach (var item in m.Meshes)
                {
                    Meshes.Add(new Mesh(item));
                }
            }
            else
            {
                _logger.Info($"Importing file");
                using AssimpContext importer = new();
                if (!importer.IsImportFormatSupported(Path.GetExtension(_path)))
                {
                    throw new ArgumentException("Model format " + Path.GetExtension(_path) + " is not supported!  Cannot load {1}", "filename");
                }
                LogStream.IsVerboseLoggingEnabled = true;
                var logger = new ConsoleLogStream();
                logger.Attach();
                Scene scene = importer.ImportFile(_path, PostProcessPreset.TargetRealTimeMaximumQuality
                    | PostProcessSteps.FlipUVs
                    | PostProcessSteps.CalculateTangentSpace
                    | PostProcessSteps.Triangulate
                    | PostProcessSteps.GlobalScale);
                Animations = new Dictionary<string, Animate.Animation>();

                ProcessNode(scene.RootNode, scene);
                _models.Add(_path, this);
                for (int i = 0; i < scene.AnimationCount; i++)
                {
                    Animations.Add(scene.Animations[i].Name.ToString(), new Animate.Animation(_path, this, i));
                }
                Animator = new Animator(Animations.FirstOrDefault().Value);
                logger.Detach();
                _logger.Info("End importing");
            }
        }
        /// <summary>
        /// Извлекаем веса вершины и устанавливаем в соответствии с словарем костей
        /// </summary>
        /// <param name="vertices">список вершин</param>
        /// <param name="mesh">меш из загружаемой модели</param>
        private void ExtractBoneWeightForVertices(List<Vertex> vertices, Assimp.Mesh mesh)
        {
            for (int boneIndex = 0; boneIndex < mesh.BoneCount; boneIndex++)
            {
                string boneName = mesh.Bones[boneIndex].Name;
                int boneID;
                if (!_boneInfoMap.ContainsKey(boneName))
                {
                    BoneInfo newBoneInfo;

                    newBoneInfo.ID = _boneCounter;
                    newBoneInfo.offset = mesh.Bones[boneIndex].OffsetMatrix.GetMatrix4FromAssimpMatrix();
                    boneID = _boneCounter;
                    _boneCounter++;
                    _boneInfoMap.Add(boneName, newBoneInfo);
                }
                else
                {
                    boneID = _boneInfoMap[boneName].ID;
                }
                var weights = mesh.Bones[boneIndex].VertexWeights;
                int numWeights = mesh.Bones[boneIndex].VertexWeightCount;
                for (int weightIndex = 0; weightIndex < numWeights; ++weightIndex)
                {
                    int vertexId = weights[weightIndex].VertexID;
                    float weight = weights[weightIndex].Weight;
                    vertices[vertexId] = SetVertexBoneData(vertices[vertexId], boneID, weight);
                }
            }
        }
        private static Vertex SetVertexBoneData(Vertex v, int boneID, float weight)
        {
            for (int i = 0; i < Vertex.MAX_BONE_INFLUENCE; i++)
            {
                if (v.BoneIDs[i] < 0)
                {
                    v.Weights[i] = weight;
                    v.BoneIDs[i] = boneID;
                    return v;
                }
            }
            return v;
        }

        public void Dispose()
        {
            foreach (var item in Meshes)
            {
                item.Dispose();
            }
        }



        #endregion
        
        #endregion
    }
}

