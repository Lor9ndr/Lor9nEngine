using System.Drawing;
using System.Drawing.Imaging;

using Lor9nEngine.Rendering.Base.Buffers;
using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Lor9nEngine.Rendering.Textures
{
    public class CubeMap : ITexture
    {
        private int _handle;
        public TextureType Type { get; set; } = TextureType.SkyBox;
        public string Path { get; set; } = Directory.GetCurrentDirectory();
        public int Handle { get => _handle; set => _handle = value; }
        public PBO[] PBOs { get; set; }
        public TextureTarget Target { get; set; }
        public Vector2i Size { get; set; }
        public int DataSize { get => Size.X * Size.Y * Channels; }
        public int Channels { get => 6; }
        public PBO[] Pbos { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IntPtr Pointer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int CurrentPboIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TextureUnit SelfUnit => TextureUnit.Texture31;
        public CubeMap(TextureType type)
        {
            EngineGL.Instance.GenTexture(out _handle);
            Type = type;
            Target = TextureTarget.TextureCubeMap;
        }
        public CubeMap(int handle, TextureType type, TextureTarget target = TextureTarget.TextureCubeMap)
        {
            _handle = handle;
            Type = type;
            Target = target;
        }
        public void SetTexParameters(Vector2i size, PixelInternalFormat format, PixelType type)
        {
            Bind();
            for (int i = 0; i < 6; i++)
            {
                EngineGL.Instance.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                    0, format, size.X, size.Y, 0, (PixelFormat)format, type, (IntPtr)null);
            }

            EngineGL.Instance.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest)
            .TexParameter(Target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest)
            .TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge)
            .TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge)
            .TexParameter(Target, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        }

        public static CubeMap LoadCubeMapFromFile(string[] faces, TextureType type)
        {
            EngineGL.Instance.GenTexture(out int handle)
                .BindTexture(TextureTarget.TextureCubeMap, handle);
            Vector2i size = new Vector2i();
            for (int i = 0; i < faces.Length; i++)
            {
                string path = Game.SKYBOX_TEXTURES_PATH + faces[i];
                // Load the image
                using var image = new Bitmap(path);

                var data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                EngineGL.Instance.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                  0,
                  PixelInternalFormat.Rgba,
                  image.Width,
                  image.Height,
                  0,
                  PixelFormat.Bgra,
                  PixelType.UnsignedByte,
                  data.Scan0);
                Vector2i currentSize = new Vector2i(image.Width, image.Height);
                if (size.EuclideanLength < currentSize.EuclideanLength)
                {
                    size = currentSize;
                }

            }

            EngineGL.Instance.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear)
                        .TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear)
                        .TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge)
                        .TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge)
                        .TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            var cubeMap = new CubeMap(handle, type, TextureTarget.TextureCubeMap) { Size = size };
            return cubeMap;
        }

        public void Bind(TextureTarget type = default) => GL.BindTexture(TextureTarget.TextureCubeMap, Handle);

        public void Bind()
        {
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);

        }

        /// <summary>
        /// Освобождение памяти
        /// </summary>
        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }
    }
}
