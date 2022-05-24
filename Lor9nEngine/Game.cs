using System.ComponentModel;
using System.Reflection;

using ImGuiNET;

using Lor9nEngine.Components.Light;
using Lor9nEngine.Components.Transform;
using Lor9nEngine.GameObjects;
using Lor9nEngine.GameObjects.Lights;
using Lor9nEngine.Input.Keyboard;
using Lor9nEngine.Rendering;
using Lor9nEngine.Scenes;

using NLog;

using OpenTK.Graphics.OpenGL4;
using OpenTK.ImGui;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lor9nEngine
{
    public class Game : GameWindow
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Ширина окна
        /// </summary>
        public static int Width { get => _width; set => _width = value; }

        /// <summary>
        /// Высота окна
        /// </summary>
        public static int Height { get => _height; set => _height = value; }
        /// <summary>
        /// Кадры в секунду
        /// </summary>
        public static float FPS { get; private set; }

        /// <summary>
        /// Разница во времени между обновлением
        /// </summary>
        public static float DeltaTime => _deltaTime;

        /// <summary>
        /// Время приложения
        /// </summary>
        public static double Time => _time;
        public static Keyboard? Keyboard { get; internal set; }

        public Shader MainShader;
        public Shader LightBoxShader;

        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public static CancellationToken CancellationToken { get; private set; }

        public List<IGameObject> GameObjects { get; private set; } = new List<IGameObject>();

        public bool EnableLight = true;

        public static Camera? Camera;
        public static Thread? UploadThread;

        #region Constants
        private static string ASSEMBLY_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!.ToString();
        public const string RESOURCE_PATH = "../../../Resources/";
        public const string TEXTURES_PATH = RESOURCE_PATH + "Textures/";
        public const string OBJ_PATH = RESOURCE_PATH + "Objects/";
        public const string NANOSUIT_PATH = OBJ_PATH + "NanoSuit/nanosuit.obj";
        public const string BRIDGE_PATH = OBJ_PATH + "Bridge/Jaaninoja_Bridge_decimated_SF.obj";
        public const string MAN_PATH = OBJ_PATH + "Man/man.fbx";
        public const string TERRAIN_PATH = OBJ_PATH + "Terrain/terrain1.fbx";
        public const string SKYBOX_TEXTURES_PATH = TEXTURES_PATH + "SkyBox/";
        public const string WORKPLACE_OBJ = OBJ_PATH + "WorkPlace/textured_output.obj";

        public const string SHADERS_PATH = "../../../Shaders/";
        public const string DEFAULT_SHADER = SHADERS_PATH + "DefaultShader";
        public const string DEFERRED_RENDER_PATH = SHADERS_PATH + "DeferredShading/";
        public const string ANOTHER_SHADERS_PATH = DEFERRED_RENDER_PATH + "Another/";
        public const string SKYBOX_SHADER_PATH = SHADERS_PATH + "SkyBox/SkyBox";
        public const string SHADOW_SHADERS_PATH = SHADERS_PATH + "Shadow/";
        #endregion

        private double oldTimeSinceStart;
        private static float _framesPerSecond;
        private static float _lastTime;
        private static double _time;
        private static float _deltaTime;
        private static int _height = 1080;
        private static int _width = 1920;
        private static Thread? UpdateAsync;
        private ImGuiController Gui;
        private int _selectedObj;
        private int _selectedMesh;
        private bool _opened;
        private bool _isGuiVisible;
        private string _sceneName = "default";
        private Scene _currentScene;
        private int _currentTexture;
        private IGLFWGraphicsContext context { get; set; }

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, new NativeWindowSettings() { APIVersion = new Version(4, 6), Profile = ContextProfile.Core })
        {
        }
        protected override void OnLoad()
        {

            Gui = new ImGuiController(this);
            Keyboard = new Keyboard(this);
            EngineGL.Instance.DebugMessageCallback(Logger.DebugProcCallback, IntPtr.Zero)
                    .Enable(EnableCap.DebugOutput)
                    .Enable(EnableCap.DebugOutputSynchronous)
                    .Enable(EnableCap.FramebufferSrgb);
            context = this.Context;
            HandleKeyboard();
            CursorState = CursorState.Grabbed;
            this.Size = new Vector2i(Width, Height);
            VSync = VSyncMode.Off;
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;

            UpdateAsync = new Thread(async () =>
            {
                _logger.Info("Starting async processes");
                while (!CancellationToken.IsCancellationRequested)
                {
                    List<Task> tasks = new List<Task>();

                    for (int i = 0; i < GameObjects.Count; i++)
                    {
                        tasks.Add(GameObjects[i].UpdateAsync());
                    }
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                    Thread.Sleep(1);
                }
                _logger.Info("End async Process");


            });

            MainShader = new Shader(DEFAULT_SHADER + ".vert", DEFAULT_SHADER + ".frag");
            LightBoxShader = new Shader(SHADERS_PATH + "LightBoxShader/LightBox.vert", SHADERS_PATH + "LightBoxShader/LightBox.frag");

            base.OnLoad();

        }
        public void LoadScene(Scene? scene)
        {
            if (scene == null)
            {
                _logger.Info("Loading default objects");
                GameObjects.Clear();
                Camera = new Camera(new Vector3(10), Size.X / (float)Size.Y);
                Camera.Enable(this);
                GameObjects.Add(Camera);
                GameObjects.Add(new SkyBox());

                var tr = new Transform(new Vector3(-10, -10, 10), new Vector3(0), new Vector3(0.2f));
                GameObjects.Add(new GameObject(ModelFactory.GetTerrainModel(), tr));
                GameObjects.Add(new GameObject(ModelFactory.GetDancingVampire(), new Transform(new Vector3(0), new Vector3(0), new Vector3(10))));
/*              GameObjects.Add(new DirectLight(new LightData(new Vector3(0.25f), new Vector3(0.5f), new Vector3(0.5f), new Vector3(1)), new Vector3(1), ModelFactory.GetLightModel()));
                GameObjects.Add(new SpotLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                GameObjects.Add(new SpotLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                GameObjects.Add(new SpotLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                GameObjects.Add(new SpotLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));*/
                GameObjects.Add(new PointLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                GameObjects.Add(new PointLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                GameObjects.Add(new PointLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                GameObjects.Add(new PointLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                GameObjects.Add(new PointLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                GameObjects.Add(new PointLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                GameObjects.Add(new PointLight(new LightConstants(), new LightData(), ModelFactory.GetLightModel()));
                _currentScene = new Scene("Default", GameObjects);
                _logger.Info("Loading models ended");
            }
            else
            {
                GameObjects = scene.GameObjects;
                _currentScene = scene;
            }
            UpdateAsync?.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {


            _currentScene.RenderAll(MainShader);

            // DEBUG RENDER LIGHT GAMEOBJECT SHADER
            _currentScene.RenderLightBoxes(LightBoxShader);


            if (_isGuiVisible)
            {
                onDrawGUI();
                Gui.Render();
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            CalculateFPS();
            Title = "FPS: " + FPS;
            base.OnUpdateFrame(args);
            Gui.Update(this, DeltaTime);

            foreach (var item in GameObjects)
            {
                item.Update();
            }
            int nrPointLights = GameObjects.OfType<PointLight>().Count();
            int nrSpotLights = GameObjects.OfType<SpotLight>().Count();
            MainShader.Use();
            MainShader.SetInt("nrPointLights", nrPointLights);
            MainShader.SetInt("nrSpotLights", nrSpotLights);
            EngineGL.Instance.UseShader(MainShader).SetShaderData("enableLight", EnableLight);

            _time += args.Time;
            _deltaTime = (float)_time - (float)oldTimeSinceStart;
            oldTimeSinceStart = _time;
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            // Update the opengl viewport
            Width = Size.X;
            Height = Size.Y;
            EngineGL.Instance.Viewport(0, 0, Width, Height);
            if (_currentScene is not null)
            {
                _currentScene.DefaultFBO.Size = e.Size;
            }
            // Tell ImGui of the new size
            Gui.WindowResized(Width, Height);
            base.OnResize(e);
        }
        protected override void OnUnload()
        {
            CancellationTokenSource.Cancel();
            base.OnUnload();

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            foreach (var item in GameObjects)
            {
                item.Dispose();
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            Gui.MouseScroll(e.Offset);
        }

        private static void CalculateFPS()
        {
            float currentTime = (float)(_time);

            _framesPerSecond++;
            if ((currentTime - _lastTime) > 1.0f)
            {
                _lastTime = currentTime;

                FPS = _framesPerSecond;

                _framesPerSecond = 0;
            }
        }
        private void HandleKeyboard()
        {
            Keyboard!.BindKey(Keys.Escape, Close, PressType.Down);
            Keyboard.BindKey(Keys.M, new Action(() => { EngineGL.Instance.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point); }), PressType.Down);
            Keyboard.BindKey(Keys.Comma, new Action(() => { EngineGL.Instance.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); }), PressType.Down);
            Keyboard.BindKey(Keys.Period, new Action(() => { EngineGL.Instance.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill); }), PressType.Down);
            Keyboard.BindKey(Keys.X, ToggleFullscreen, PressType.Down);
            Keyboard.BindKey(Keys.LeftControl, new Action(() =>
            {
                CursorState = CursorState.Normal;
                _isGuiVisible = true;
                Camera.CanMove = false;
            }), PressType.Down);
            Keyboard.BindKey(Keys.O, new Action(() =>
            {
                var light = GameObjects.OfType<SpotLight>().FirstOrDefault();
                light.Transform.Position = Camera.Transform.Position;
                light.Transform.Direction = Camera.Transform.Direction;
            }), PressType.Down);

            Keyboard.BindKey(Keys.LeftAlt, new Action(() =>
            {
                CursorState = CursorState.Grabbed;
                Camera.CanMove = true;
                _isGuiVisible = false;

            }), PressType.Down);
        }

        private void ToggleFullscreen()
        {
            if (IsFullscreen)
            {
                WindowBorder = WindowBorder.Fixed;
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowBorder = WindowBorder.Hidden;
                WindowState = WindowState.Fullscreen;
            }
            Thread.Sleep(360);
            Camera.AspectRatio = Size.X / (float)Size.Y;
        }
        void onDrawGUI()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.ShowStyleSelector("Style"))
                {
                }

                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open", "CTRL+O"))
                    {
                        _currentScene = SceneLoader.LoadScene("default");
                    }

                    ImGui.Separator();
                    if (ImGui.MenuItem("Save", "CTRL+S"))
                    {
                    }

                    if (ImGui.MenuItem("Save As", "CTRL+Shift+S"))
                    {
                        SceneLoader.SaveScene(new Scene(_sceneName, GameObjects));
                    }

                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("CheckBoxes"))
                {

                    ImGui.Checkbox("EnableLight", ref EnableLight);
                    ImGui.EndMenu();

                }



                ImGui.EndMainMenuBar();
            }


            List<IGameObject> gos = new List<IGameObject>();
            gos.AddRange(GameObjects);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(Size.X - 280, 20));
            if (ImGui.Begin("Objects"))
            {
                ImGui.SetNextWindowSize(new System.Numerics.Vector2(100, gos.Count * 30));
                ImGui.ListBox("Objects ", ref _selectedObj, gos.Select(s => s.GetType().ToString().Split('.').Last()).ToArray(), gos.Count, 30);
            }

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(Size.X / 2 + 360, 20));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(300, 900), ImGuiCond.Always);
            var obj = gos[_selectedObj];
            if (obj is SkyBox)
            {

            }

            else if (ImGui.Begin("Properties", ref _opened, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.AlwaysUseWindowPadding))
            {
                ImGui.Text("Transform");

                Vector3 pos = obj.Transform.Position;
                var p = new System.Numerics.Vector3(pos.X, pos.Y, pos.Z);
                ImGui.DragFloat3("Position", ref p, 0.1f);
                obj.Transform.Position = new Vector3(p.X, p.Y, p.Z);

                Vector3 rotation = obj.Transform.Rotation;
                var r = new System.Numerics.Vector3(rotation.X, rotation.Y, rotation.Z);
                ImGui.DragFloat3("Rotation", ref r, 0.1f);
                obj.Transform.Rotation = new Vector3(r.X, r.Y, r.Z);

                Vector3 scale = obj.Transform.Scale;
                var s = new System.Numerics.Vector3(scale.X, scale.Y, scale.Z);
                ImGui.DragFloat3("Scale", ref s, 0.1f);
                obj.Transform.Scale = new Vector3(s.X, s.Y, s.Z);

                Vector3 direction = obj.Transform.Direction;
                var direct = new System.Numerics.Vector3(direction.X, direction.Y, direction.Z);
                ImGui.DragFloat3("Direction", ref direct, 0.1f);
                obj.Transform.Direction = new Vector3(direct.X, direct.Y, direct.Z);


                if (obj is Camera)
                {
                    Camera cam = (Camera)obj;
                    float fov = cam.Fov;
                    ImGui.DragFloat("FOV", ref fov, 0.1f);
                    cam.Fov = fov;

                }
                else
                {
                    if (obj is BaseLight)
                    {
                        var light = (BaseLight)obj;
                        ImGui.Text("LightData");

                        Vector3 ambient = light.LightData.Ambient;
                        var a = new System.Numerics.Vector3(ambient.X, ambient.Y, ambient.Z);
                        ImGui.DragFloat3("Ambient", ref a, 0.1f);
                        light.LightData.Ambient = new Vector3(a.X, a.Y, a.Z);


                        Vector3 diffuse = light.LightData.Diffuse;
                        var d = new System.Numerics.Vector3(diffuse.X, diffuse.Y, diffuse.Z);
                        ImGui.DragFloat3("Diffuse", ref d, 0.1f);
                        light.LightData.Diffuse = new Vector3(d.X, d.Y, d.Z);

                        Vector3 specular = light.LightData.Specular;
                        var spec = new System.Numerics.Vector3(specular.X, specular.Y, specular.Z);
                        ImGui.DragFloat3("Specular", ref spec, 0.1f);
                        light.LightData.Specular = new Vector3(spec.X, spec.Y, spec.Z);

                        Vector3 color = light.LightData.Color;
                        var c = new System.Numerics.Vector3(color.X, color.Y, color.Z);
                        ImGui.DragFloat3("Color", ref c, 0.1f);
                        light.LightData.Color = new Vector3(c.X, c.Y, c.Z);


                        float intensity = light.LightData.Intensity;
                        ImGui.DragFloat("Intensity", ref intensity, 0.1f);
                        light.LightData.Intensity = intensity;
                        if (obj is IConstantableLight constantableLight)
                        {
                            ImGui.Text("Light constants");
                            float constant = constantableLight.LightConstants.Constant;
                            ImGui.DragFloat("Constant", ref constant, 0.1f);
                            constantableLight.LightConstants.Constant = constant;

                            float linear = constantableLight.LightConstants.Linear;
                            ImGui.DragFloat("Linear", ref linear, 0.1f);
                            constantableLight.LightConstants.Linear = linear;

                            float quadratic = constantableLight.LightConstants.Quadratic;
                            ImGui.DragFloat("Quadratic", ref quadratic, 0.1f);
                            constantableLight.LightConstants.Quadratic = quadratic;
                        }

                        float far = light.Shadow.Far;
                        ImGui.DragFloat("Far", ref far, 0.1f);
                        light.Shadow.Far = far;


                        float near = light.Shadow.Near;
                        ImGui.DragFloat("Near", ref near, 0.1f);
                        light.Shadow.Near = near;


                    }
                    ImGui.ListBox("Meshes", ref _selectedMesh, obj.Model.Meshes.Select(s => s.Name).ToArray(), obj.Model.Meshes.Count, 30);
                    if (obj.Model.Meshes.Count - 1 < _selectedMesh)
                    {
                        _selectedMesh = 0;
                    }

                }

            }
            if (obj.Model != null && obj is not SkyBox)
            {
                var currentMesh = obj.Model.Meshes[_selectedMesh];
                if (ImGui.ListBox("Textures", ref _currentTexture, currentMesh.Textures.Select(s => s.Path.Split("/").Last()).ToArray(), currentMesh.Textures.Count, 30))
                {
                }
                if (ImGui.Begin("Texture"))
                {
                    ImGui.Image((IntPtr)currentMesh.Textures[_currentTexture].Handle, new System.Numerics.Vector2(200));
                }
            }
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(Size.X / 2 - 360, 20));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(300, 900), ImGuiCond.Always);
            if (ImGui.Begin("Add Lights"))
            {
                if (ImGui.Button("Add spotLight"))
                {
                    GameObjects.Add(new SpotLight(new LightConstants(), new LightData(new Vector3(0.25f), new Vector3(1.0f), new Vector3(0.3f), new Vector3(1)), ModelFactory.GetLightModel()));
                }
                if (ImGui.Button("Add Point Light"))
                {
                    GameObjects.Add(new PointLight(new LightConstants(), new LightData(new Vector3(0.25f), new Vector3(1.0f), new Vector3(0.3f), new Vector3(1)), ModelFactory.GetLightModel()));
                }
            }
            
            ImGui.End();
        }
    }
}
