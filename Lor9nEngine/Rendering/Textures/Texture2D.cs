
using System.Drawing;
using System.Drawing.Imaging;

using Lor9nEngine.Rendering.Base.Buffers;
using Lor9nEngine.Rendering.Interfaces;

using Newtonsoft.Json;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;


namespace Lor9nEngine.Rendering.Textures
{
    /// <summary>
    /// Класс текстур
    /// </summary>
    internal class Texture2D : ITexture
    {
        private Vector2i _size = new Vector2i(800, 600);

        private int _handle;
        private PBO[] _pbos;

        public TextureType Type { get; set; } = TextureType.Diffuse;
        public string Path { get; set; } = Directory.GetCurrentDirectory();
        public int Handle { get => _handle; set => _handle = value; }
        public TextureTarget Target { get; set; } = TextureTarget.Texture2D;

        public int DataSize => Size.X * Size.Y * Channels;

        public int Channels => 4;

        public TextureUnit SelfUnit => TextureUnit.Texture10;

        public Vector2i Size { get => _size; set => _size = value; }
        public IntPtr Pointer { get; set; }
        public PBO[] Pbos { get => _pbos; set => _pbos = value; }

        public int CurrentPboIndex { get; set; }

        /// <summary>
        /// Конструктор текстуры, где сразу определяется идентификатор текстуры
        /// </summary>
        public Texture2D(TextureType type)
        {
            Type = type;
            EngineGL.Instance.GenTexture(out _handle);
        }
        /// <summary>
        /// Основной конструктор тектсуры
        /// </summary>
        /// <param name="id">Идентификатор текстуры</param>
        /// <param name="type">Тип текстуры</param>
        /// <param name="path">Путь текстуры</param>
        public Texture2D(int id, TextureType type, string path)
            : this(id, type) => Path = path;
        public Texture2D(int id, TextureType type, string path, TextureTarget target = TextureTarget.Texture2D)
        : this(id, type, path) => Target = target;

        /// <summary>
        /// Базовый конструктор текстуры, создаваемый только с помощью идентификтора текстуры
        /// </summary>
        /// <param name="handle">Идентификтор текстуры</param>
        public Texture2D(int handle)
            => _handle = handle;


        /// <summary>
        /// Базовый конструктор текстуры, создаваемый только с помощью идентификтора и типом текстуры
        /// </summary>
        /// <param name="handle">Идентификтор текстуры</param>
        /// <param name="type">Тип текстуры</param>
        public Texture2D(int handle, TextureType type)
            : this(handle) => Type = type;

        public Texture2D(int handle, TextureType type, string path, Vector2i size) : this(handle, type, path)
        {
            Size = size;
        }

        [JsonConstructor]
        public Texture2D(string path, TextureType type, string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                Path = directory + '/' + path;
            }
            // Generate handle


            // Load the image
            try
            {
                var pointer = IntPtr.Zero;
                using var image = ITexture.GetData(path, out BitmapData data);

                EngineGL.Instance.GenTexture(out _handle);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.SrgbAlpha, image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                EngineGL.Instance.GenerateMipmap(GenerateMipmapTarget.Texture2D)
                      .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear)
                      .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear)
                      .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat)
                      .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat)
                      ;
                GL.BindTexture(TextureTarget.Texture2D, 0);

                Size = new Vector2i(image.Width, image.Height);
                Type = type;
                Pointer = data.Scan0;
            }
            catch (ArgumentException)
            {
                var tex = (Texture2D)GetDefaultTexture;
                Size = tex.Size;
                Handle = tex.Handle;
                Path = tex.Path;
            }
        }

        /// <summary>
        /// Загузка и создание текстуры из файла
        /// </summary>
        /// <param name="path">Путь к текстуре</param>
        /// <param name="type">Тип текстуры</param>
        /// <param name="directory">Директория текстуры</param>
        /// <returns></returns>
        public static ITexture LoadFromFile(string path, TextureType type, string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                path = directory + '/' + path;
            }
            // Generate handle
            EngineGL.Instance.GenTexture(out int handle);
            Vector2i size;
            IntPtr sync;
            // Load the image
            try
            {
                using (Bitmap image = new Bitmap(path))
                {
                    // TODO: Получать intpr из текстуры с помощью другого инструмента;
                    BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                    size = new Vector2i(image.Width, image.Height);
                    var pointer = data.Scan0;
                    GL.BindTexture(TextureTarget.Texture2D, handle);

                    GL.TexImage2D(TextureTarget.Texture2D,
                                0,
                                PixelInternalFormat.SrgbAlpha,
                                image.Width,
                                image.Height,
                                0,
                                PixelFormat.Bgra,
                                PixelType.UnsignedByte,
                                pointer);



                    EngineGL.Instance.GenerateMipmap(GenerateMipmapTarget.Texture2D)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat)
                          ;
                    var tex = new Texture2D(handle, type, path) { Pointer = pointer };
                    return tex;
                }
            }
            catch (ArgumentException)
            {
                return GetDefaultTexture;
            }
        }

        public static ITexture LoadWithPBO(string path, TextureType type, string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                path = directory + '/' + path;
            }
            // Generate handle
            EngineGL.Instance.GenTexture(out int handle);
            Vector2i size;
            IntPtr sync;
            // Load the image
            try
            {
                using (Bitmap image = new Bitmap(path))
                {
                    // TODO: Получать intpr из текстуры с помощью другого инструмента;
                    BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                    size = new Vector2i(image.Width, image.Height);
                    var pointer = data.Scan0;
                    PBO[] pbos = new PBO[ITexture.MAX_PBOS];
                    GL.BindTexture(TextureTarget.Texture2D, handle);

                    for (int i = 0; i < ITexture.MAX_PBOS; i++)
                    {
                        pbos[i].Setup(size.X * size.Y * 4, pointer);

                        GL.TexImage2D(TextureTarget.Texture2D,
                                    0,
                                    PixelInternalFormat.SrgbAlpha,
                                    image.Width,
                                    image.Height,
                                    0,
                                    PixelFormat.Bgra,
                                    PixelType.UnsignedByte,
                                    (IntPtr)null);
                    }

                    EngineGL.Instance.GenerateMipmap(GenerateMipmapTarget.Texture2D)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat)
                          ;
                    var tex = new Texture2D(handle, type, path) { Pbos = pbos, Pointer = pointer };
                    return tex;
                }
            }
            catch (ArgumentException)
            {
                return GetDefaultTexture;
            }
        }

        /// <summary>
        /// Привязка текстуры к следующему рендерируемому объекту
        /// </summary>
        /// <param name="target">Целевой тип текстуры</param>
        public void Bind(TextureTarget target)
            => EngineGL.Instance.BindTexture(target, Handle);
        /// <summary>
        /// Приатачить текстуру к юниту и забиндить её 
        /// </summary>
        /// <param name="unit">На какой юнит коннектить текстуру</param>
        /// <param name="taget">Целевой тип текстуры</param>
        public void Use(TextureUnit unit, TextureTarget taget)
            => EngineGL.Instance.ActiveTexture(unit).BindTexture(taget, Handle);

        /// <summary>
        /// Отвязка текстуры
        /// </summary>
        /// <param name="target">От какой цели отвзяываем</param>
        public void Unbind(TextureTarget target)
            => EngineGL.Instance.BindTexture(target, 0);

        /// <summary>
        /// Приатачить текстуру к юниту и забиндить её 
        /// </summary>
        /// <param name="unit">На какой юнит коннектить текстуру</param>
        public void Use(TextureUnit unit)
            => EngineGL.Instance.ActiveTexture(unit).BindTexture(Target, this);

        /// <summary>
        /// Обычная diffuse текстура
        /// </summary>
        public static ITexture GetDefaultTexture = LoadFromFile(Game.TEXTURES_PATH + "/DefaultBase.jpg", TextureType.Diffuse, string.Empty);

        /// <summary>
        /// Пустая текстура нормалей
        /// </summary>
        public static ITexture GetDefaultNormalTexture = LoadFromFile(Game.TEXTURES_PATH + "/EmptyNormal.png", TextureType.Normal, string.Empty);

        /// <summary>
        /// Список текстуры цвета и нормалей
        /// </summary>
        public static List<ITexture> GetDefaultTextures = new List<ITexture>() { GetDefaultTexture, GetDefaultNormalTexture };

        public static bool operator ==(Texture2D tex1, Texture2D tex2)
        {
            return tex1.Equals(tex2.Handle);
        }
        public static bool operator !=(Texture2D tex1, Texture2D tex2)
        {
            return !tex1.Equals(tex2.Handle);
        }

        public override bool Equals(object obj)
        {
            return obj is ITexture && Handle.Equals(((ITexture)obj).Handle);
        }



        public void Bind() => Bind(TextureTarget.Texture2D);

        public void Unbind() => Unbind(TextureTarget.Texture2D);


        /// <summary>
        /// Освобождение памяти
        /// </summary>
        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }

        public override int GetHashCode() => Handle.GetHashCode();


        public void SetTexParameters(Vector2i size, PixelInternalFormat format, PixelFormat anotherFormat, PixelType type)
        {
            Bind(Target);
            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            EngineGL.Instance.TexImage2D(TextureTarget.Texture2D, 0, format, size.X, size.Y, 0, PixelFormat.DepthComponent, type, (IntPtr)null)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder)
                        .GenerateMipmap(GenerateMipmapTarget.Texture2D)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, 0);
            Unbind();
        }
    }
}
