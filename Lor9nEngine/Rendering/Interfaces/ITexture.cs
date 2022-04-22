using Lor9nEngine.Rendering.Base.Buffers;
using Lor9nEngine.Rendering.Textures;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Lor9nEngine.Rendering.Interfaces
{
    internal interface ITexture : IGLObject
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
        /// <param name="taget">Целевой тип текстуры</param>
        public void Use(TextureUnit unit, TextureTarget taget)
            => EngineGL.Instance.ActiveTexture(unit).BindTexture(taget, this);

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
        public void SetTexParameters(Vector2i size, PixelInternalFormat format, PixelType type)
            => SetTexParameters(size, format, (PixelFormat)format, type);

        /// <summary>
        /// Установка параметров текстуры
        /// </summary>
        /// <param name="size">Размер</param>
        /// <param name="format">Формат пикселей</param>
        /// <param name="type">тип пикселей</param>
        /// <param name="anotherFormat">Формат пикселей</param>
        public void SetTexParameters(Vector2i size, PixelInternalFormat format, PixelFormat anotherFormat, PixelType type)
        {
            Bind(TextureTarget.Texture2D);
            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            EngineGL.Instance.TexImage2D(TextureTarget.Texture2D, 0, format, size.X, size.Y, 0, anotherFormat, type, (IntPtr)null)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder)
                        .GenerateMipmap(GenerateMipmapTarget.Texture2D)
                        .TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, 0);
        }
    }
}
