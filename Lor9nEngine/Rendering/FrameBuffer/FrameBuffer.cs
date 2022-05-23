using Lor9nEngine.Rendering.Interfaces;
using Lor9nEngine.Rendering.Textures;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Lor9nEngine.Rendering.FrameBuffer
{
    public class FrameBuffer : IGLObject, ISettupable
    {
        private Vector2i _size;
        private readonly ClearBufferMask _bufferMask;
        private int _handle;
        private ITexture _texture;

        /// <summary>
        /// Идентификатор буффера
        /// </summary>
        public int Handle { get => _handle; set => _handle = value; }

        /// <summary>
        /// Размер буффера
        /// </summary>
        public Vector2i Size
        {
            get
            {
                return _size;
            }
            set
            {
                if (value != _size)
                {
                    _size = value;
                    OnResize.Invoke(new ResizeEventArgs(_size));
                }
            }
        }

        /// <summary>
        /// Текстура буффера
        /// </summary>
        public ITexture Texture { get => _texture; }

        public event Action<ResizeEventArgs> OnResize;


        /// <summary>
        /// Конструктор буффера кадра
        /// </summary>
        /// <param name="size">Размер буффера</param>
        /// <param name="bufferMask">Какую маску очищать нужно</param>
        public FrameBuffer(Vector2i size, ClearBufferMask bufferMask)
        {
            _size = size;
            _bufferMask = bufferMask;
            Handle = 0;
            OnResize += Resize;
        }

        protected virtual void Resize(ResizeEventArgs e)
        {
            this.Size = e.Size;
            SetViewPort();
        }

        /// <summary>
        /// Привязываем буффер кадра
        /// </summary>
        public void Bind()
        {
            //Console.WriteLine($"BINDING FBO {Handle}");
            EngineGL.Instance.BindFramebuffer(FramebufferTarget.Framebuffer, this);
        }

        /// <summary>
        /// Отвязываем буффер кадра
        /// </summary>
        public void Unbind()
        {
            //Console.WriteLine($"UNBINDING FBO {Handle}");

            EngineGL.Instance.BindFramebuffer(FramebufferTarget.Framebuffer, EmptyFrameBuffer);
        }

        /// <summary>
        /// Настройка Фреймбуффера - его генерация
        /// </summary>
        public void Setup() => EngineGL.Instance.GenFramebuffer(out _handle);

        /// <summary>
        /// Очистка маски буффера кадра
        /// </summary>
        public void Clear() => EngineGL.Instance.Clear(_bufferMask);

        /// <summary>
        /// Активация буффера
        /// </summary>
        public void Activate()
        {
            Bind();
            SetViewPort();
            Clear();
        }

        /* /// <summary>
         /// Отображать буффер в текущий буффер
         /// В основном для отладки,но рекомендую юзать приложение RenderDoc
         /// </summary>
         /// <param name="texID">какую текстуру отображаем</param>
         public void DisplayFrameBufferTexture(int texID)
         {
             _debugShader.Use();
             EngineGL.Instance.ActiveTexture(TextureUnit.Texture0).BindTexture(TextureTarget.Texture2D, texID);
             _debugQuad.Draw(PrimitiveType.TrianglesAdjacency);
         }*/

        /// <summary>
        /// Проверка состояния буффера кадра, при создании самого буффера кадра
        /// </summary>
        /// <returns><see langword="true"/> если все хорошо, <see langword="false"/> если все плохо)</returns>
        /// <exception cref="Exception">ошибка,если буффер кадра не создался</exception>
        public bool CheckState()
        {
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferComplete)
            {
                Unbind();
                return true;
            }
            else
            {
                throw new Exception(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer).ToString());
            }
        }

        /// <summary>
        /// Установка вьюпорта буффера кадра
        /// </summary>
        public void SetViewPort() => EngineGL.Instance.Viewport(0, 0, Size.X, Size.Y);

        /// <summary>
        /// Привязка кубической текстуры
        /// </summary>
        /// <param name="attachment">привязка к определенному слою вроде буффера кадра </param>
        /// <param name="format">Формат текстуры</param>
        /// <param name="type">Тип пикслей</param>
        public void AttachCubeMap(FramebufferAttachment attachment, PixelInternalFormat format, PixelType type)
        {
            _texture = new CubeMap(TextureType.Shadow) { Size = Size };
            _texture.SetTexParameters(Size, format, type);
            EngineGL.Instance.FramebufferTexture(FramebufferTarget.Framebuffer, attachment, _texture, 0);
            CheckState();

        }

        /// <summary>
        /// Привязка 2D текстуры
        /// </summary>
        /// <param name="attachment">привязка к определенному слою вроде буффера кадра </param>
        /// <param name="format">Формат текстуры</param>
        /// <param name="type">Тип пикслей</param>
        public void AttachTexture2DMap(FramebufferAttachment attachment, PixelInternalFormat format, PixelType type)
        {
            _texture = new Texture2D(TextureType.Shadow) { Size = Size };
            _texture.SetTexParameters(Size, format, type);
            EngineGL.Instance.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D, _texture, 0);
            CheckState();

        }

        /// <summary>
        /// Отключение чтения и отрисовки буффера
        /// </summary>
        public void DisableColorBuffer()
        {
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
        }

        /// <summary>
        /// Удаление буффера
        /// </summary>
        public void DeleteBuffer()
        {
            if (_texture != null)
            {
                GL.DeleteTexture(_texture.Handle);
            }
            GL.DeleteBuffer(Handle);
        }

        /// <summary>
        /// Освобождение памяти
        /// </summary>
        public void Dispose()
        {
            DeleteBuffer();
        }

        /// <summary>
        /// Пустой буффер кадра
        /// </summary>
        public static FrameBuffer EmptyFrameBuffer = new FrameBuffer(new Vector2i(0), ClearBufferMask.None) { Handle = 0 };
    }
}
