using System.Drawing;
using System.Drawing.Imaging;

using Lor9nEngine.Rendering.Base.Buffers;
using Lor9nEngine.Rendering.Textures;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Lor9nEngine.Rendering.Interfaces
{
    public interface ITexture : IGLObject
    {
        /// <summary>
        /// Тип текстуры
        /// </summary>
        public TextureType Type { get; set; }
        public TextureTarget Target { get; set; }
        /// <summary>
        /// Путь текстуры
        /// </summary>
        public string Path { get; set; }

        public const int MAX_PBOS = 2;
        public PBO[] Pbos { get; set; }
        public int CurrentPboIndex { get; set; }
        public Vector2i Size { get; set; }
        public int DataSize { get; }
        public int Channels { get; }
        public IntPtr Pointer { get; set; }
        /// <summary>
        /// Привязка текстуры к следующему рендерируемому объекту
        /// </summary>
        /// <param name="target">Целевой тип текстуры</param>
        public void Bind(TextureTarget target)
            => EngineGL.Instance.BindTexture(target, this);
        /// <summary>
        /// Приатачить текстуру к юниту и забиндить её 
        /// </summary>
        /// <param name="unit">На какой юнит коннектить текстуру</param>
        /// <param name="target">Целевой тип текстуры</param>
        public void Use(TextureUnit unit, TextureTarget target) => EngineGL.Instance.ActiveTexture(unit).BindTexture(target, this);

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
        /// Установка параметров текстуры
        /// </summary>
        /// <param name="size">Размер</param>
        /// <param name="format">Формат пикселей</param>
        /// <param name="type">тип пикселей</param>
        /// <param name="format">Формат пикселей</param>
        public void SetTexParameters(Vector2i size, PixelInternalFormat internalFormat, PixelFormat format, PixelType type);

        protected static Image GetData(string path, out BitmapData data)
        {
            var image = new Bitmap(path);
            data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            return image;
        }
    }
}
