using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base.Buffers;
using Lor9nEngine.Rendering.FrameBuffer;
using Lor9nEngine.Rendering.Interfaces;
using Lor9nEngine.Rendering.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Lor9nEngine
{
    /// <summary>
    /// Класс для работы с OpenGL
    /// </summary>
    internal class EngineGL
    {
        internal static EngineGL Instance = new EngineGL();
        private CullFaceMode _currentCullFaceMode;
        private IGLObject _bindedVao;

      

        private bool _depthMask;
        private Shader _activeShader;
        private EngineViewport _activeViewport;
        private MaterialFace _activePolygonFace = MaterialFace.FrontAndBack;


        private PolygonMode activePolygonMode = OpenTK.Graphics.OpenGL4.PolygonMode.Fill;
        private TextureUnit _activeTextureUnit;

        /// <summary>
        /// Вьюпорт
        /// </summary>
        internal struct EngineViewport
        {
            /// <summary>
            /// начальная координата X
            /// </summary>
            internal int X;

            /// <summary>
            /// начальная координата Y
            /// </summary>
            internal int Y;

            /// <summary>
            /// Ширина
            /// </summary>
            internal int Width;

            /// <summary>
            /// Высота
            /// </summary>
            internal int Height;

            /// <summary>
            /// Оператор сравнения вьюпорта
            /// </summary>
            public static bool operator ==(EngineViewport view1, EngineViewport view2) => view1.Equals(view2);

            /// <summary>
            /// Оператор сравнения вьюпорта
            /// </summary>
            public static bool operator !=(EngineViewport view1, EngineViewport view2) => !view1.Equals(view2);

            /// <summary>
            /// Функция сравнения
            /// </summary>
            public override bool Equals([NotNullWhen(true)] object obj) => GetHashCode() == obj.GetHashCode();

            /// <summary>
            /// Функция, возращающая хэшкод
            /// </summary>
            public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
            /// <summary>
            /// Базовый конструктор
            /// </summary>
            /// <param name="x">Начальная координата X</param>
            /// <param name="y">Начальная координата Y</param>
            /// <param name="width">Ширина</param>
            /// <param name="height">Высота</param>
            internal EngineViewport(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }

        #region internal Properties

        /// <summary>
        /// Отправленные в OpenGL буфферы,
        /// где ключ <see cref="BufferTarget"/> - это целевой буффер 
        /// и значение <see cref="int"/> - индекс буффера
        /// </summary>
        internal Dictionary<BufferTarget, IBufferVerticesObject> BindedBuffer = new Dictionary<BufferTarget, IBufferVerticesObject>();

        /// <summary>
        /// Текующий режим отрисовки полигонов
        /// </summary>
        internal PolygonMode ActivePolygonMode
        {
            get => activePolygonMode;
            set
            {
                if (activePolygonMode != value)
                {
                    activePolygonMode = value;
                    PolygonMode(ActivePolygonFace, value);
                }
            }
        }

        /// <summary>
        /// Текущая сторона отрисовки полигона
        /// </summary>
        internal MaterialFace ActivePolygonFace
        {
            get => _activePolygonFace;
            set
            {
                if (_activePolygonFace != value)
                {
                    _activePolygonFace = value;
                    PolygonMode(value, ActivePolygonMode);
                }
            }
        }

        /// <summary>
        /// Текущий вьюпорт
        /// </summary>
        internal EngineViewport ActiveViewport
        {
            get => _activeViewport;
            set
            {
                if (_activeViewport != value)
                {
                    _activeViewport = value;
                    Viewport(value.X, value.Y, value.Width, value.Height);
                }
            }
        }

        /// <summary>
        /// Текущее значение маски глубины
        /// </summary>
        internal bool DepthMaskValue
        {
            get => _depthMask;
            set
            {
                if (_depthMask != value)
                {
                    _depthMask = value;
                    DepthMask(value);
                }
            }
        }

        /// <summary>
        /// Текущий режим отсечения граней
        /// </summary>
        internal CullFaceMode CurrentCullFaceMode
        {
            get => _currentCullFaceMode;
            set
            {
                if (_currentCullFaceMode != value)
                {
                    _currentCullFaceMode = value;
                    CullFace(value);
                }
            }
        }

        /// <summary>
        /// Текущий объект массива вершин
        /// </summary>
        internal IGLObject? BindedVao
        {
            get => _bindedVao;
            set
            {
                if (_bindedVao != value)
                {
                    _bindedVao = value;
                    BindVAO(value);
                }
            }
        }

       

        /// <summary>
        /// Активный шейдер
        /// </summary>
        internal Shader ActiveShader
        {
            get => _activeShader;
            set
            {
                if (_activeShader != value)
                {
                    _activeShader = value;
                }
            }
        }

        /// <summary>
        /// Текущий юнит текстуры
        /// </summary>
        internal TextureUnit ActiveTextureUnit
        {
            get => _activeTextureUnit;
            set
            {
                if (_activeTextureUnit != value)
                {
                    _activeTextureUnit = value;
                    ActiveTexture(value);
                }
            }
        }

        /// <summary>
        /// Отправленный индекс текстуры может быть не равен Texture.Handle
        /// </summary>
        internal int BindedTexID { get; private set; }

        /// <summary>
        /// Отправленный класс текстуры, индекс берется из BindedTexture.Handle
        /// </summary>
        internal ITexture BindedTexture { get; private set; }

        /// <summary>
        /// Текущий буффер кадра
        /// </summary>
        internal FrameBuffer CurrentFrameBuffer { get; private set; }
        #endregion

        /// <summary>
        /// Генерируем индентификатор массива вершинного объекта 
        /// </summary>
        /// <param name="vao">возвращаемое значение</param>
        internal EngineGL GenVertexArray(out int vao)
        {
            vao = GL.GenVertexArray();
            return this;
        }

        /// <summary>
        /// включить <see cref="EnableCap"/>
        /// </summary>
        /// <param name="cap">Что включаем</param>
        internal EngineGL Enable(EnableCap cap)
        {
            GL.Enable(cap);
            return this;
        }

        /// <summary>
        /// Отключаем <see cref="EnableCap"/>
        /// </summary>
        /// <param name="cap">Что отключаем</param>
        internal EngineGL Disable(EnableCap cap)
        {
            GL.Disable(cap);
            return this;
        }

        internal EngineGL DepthFunc(DepthFunction function)
        {
            GL.DepthFunc(function);
            return this;
        }
        internal EngineGL DepthRange(float near ,float far)
        {
            GL.DepthRange(near,far);
            return this;
        }
        internal EngineGL ClearDepth(double value)
        {
            GL.ClearDepth(value);
            return this;
        }
        /// <summary>
        /// Маска цвета
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="a">alpha</param>
        internal EngineGL ColorMask(bool r, bool g, bool b, bool a)
        {
            GL.ColorMask(r, g, b, a);
            return this;
        }

        /// <summary>
        /// Режим отсечения грани
        /// </summary>
        /// <param name="mode">
        /// <code>if (mode == CullFaceMode.Front)</code> - отсекаем передние грани
        /// <code>if (mode == CullFaceMode.Back)</code> - отсекаем задние грани
        /// <code>if (mode == CullFaceMode.FrontAndBack)</code>- отсекаем и то и другое. Но я хз когда это может пригодится
        /// </param>
        internal EngineGL CullFace(CullFaceMode mode)
        {

            if (CurrentCullFaceMode != mode)
            {
                GL.CullFace(mode);
                CurrentCullFaceMode = mode;
            }
            return this;
        }

        /// <summary>
        /// Маска глубины
        /// </summary>
        /// <param name="value">включить/выключить</param>
        internal EngineGL DepthMask(bool value)
        {
            GL.DepthMask(value);
            DepthMaskValue = value;
            return this;
        }

        /// <summary>
        /// Отправка Объекта массива вершин
        /// </summary>
        /// <param name="vao">класс с индексом объекта массива вершин</param>
        internal EngineGL BindVAO(IGLObject vao)
        {
            if (BindedVao != vao)
            {
                GL.BindVertexArray(vao.Handle);
                BindedVao = vao;
            }
            return this;
        }
        internal EngineGL DeleteVAO(IGLObject vao)
        {
            GL.DeleteVertexArray(vao.Handle);
            return this;
        }

        internal EngineGL UnbindVAO()
        {

            GL.BindVertexArray(0);
            BindedVao = null;
            return this;
        }

        /// <summary>
        /// Отправка Объекта массива вершин
        /// </summary>
        /// <param name="vao">индекс объект массива вершин</param>
        internal EngineGL BindVAO(int vao = 0)
        {
            GL.BindVertexArray(vao);
            
            BindedVao = null;
            return this;
        }

        /// <summary>
        /// Отправка буффера
        /// </summary>
        /// <param name="target">Целевой буффер</param>
        /// <param name="buffer">индекс буффера</param>
        internal EngineGL BindBuffer(BufferTarget target, IBufferObject buffer)
        {
            GL.BindBuffer(target, buffer.Handle);
            return this;
        }
        internal EngineGL UnbindBuffer(BufferTarget target)
        {
            GL.BindBuffer(target,0);
            return this;
        }

        /// <summary>
        /// Отрисовка элементов, когда есть EBO буффер
        /// </summary>
        /// <param name="type">тип отрисовки</param>
        /// <param name="count">количество индексов для вершин</param>
        /// <param name="deType">тип данных индексов</param>
        /// <param name="indices">индексы, если уже забинжен VAO и EBO находится внутри VAO, то стоит указать 0</param>
        internal EngineGL DrawElements(PrimitiveType type, int count, DrawElementsType deType, int indices)
        {
            GL.DrawElements(type, count, deType, indices);
            return this;
        }

        /// <summary>
        /// Отрисовка с помощью только VAO объекта
        /// Может отрисовываться неправильно, если изначально объект был с индексами вершин,
        /// а ты собираешься отрисовывать только вершины без индексов,
        /// так как индексы помогают уменьшить кол-во вершин и повторять уже существующие вершины
        /// </summary>
        /// <param name="type">Тип отрисовки</param>
        /// <param name="first">Начиная с этой вершины,отсчет от 0</param>
        /// <param name="count">Кол-во вершин</param>
        internal EngineGL DrawArrays(PrimitiveType type, int first, int count)
        {
            GL.DrawArrays(type, first, count);
            return this;
        }

        /// <summary>
        /// Отправка данных в последний активный шейдер
        /// </summary>
        /// <typeparam name="T">Тип отправляемый в шейдер</typeparam>
        /// <param name="name">название uniform в шейдере</param>
        /// <param name="data">собственно сами данные,которые будут отправляться в шейдер</param>
        internal EngineGL SetShaderData<T>(string name, T data)
            where T : struct
        {
            if (data is Vector3 data3)
            {
                ActiveShader.SetVector3(name, data3);
            }
            else if (data is Matrix4 dataMat4)
            {
                ActiveShader.SetMatrix4(name, dataMat4);
            }
            else if (data is bool dataBool)
            {
                ActiveShader.SetInt(name, dataBool);
            }
            else if (data is int dataInt)
            {
                ActiveShader.SetInt(name, dataInt);
            }
            else if (data is float dataFloat)
            {
                ActiveShader.SetFloat(name, dataFloat);
            }
            else if (data is Vector2 data2)
            {
                ActiveShader.SetVector2(name, data2);
            }
            return this;
        }

        /// <summary>
        /// Установка цвета
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="a">alpha</param>
        internal EngineGL ClearColor(float r, float g, float b, float a)
        {
            GL.ClearColor(r, g, b, a);
            return this;
        }

        /// <summary>
        /// Включение дебаггера
        /// </summary>
        internal EngineGL DebugMessageCallback(DebugProc proc, IntPtr userPtr)
        {
            GL.DebugMessageCallback(proc, userPtr);
            return this;
        }

        /// <summary>
        /// Очистка маски буффера 
        /// </summary>
        /// <param name="mask">какую маску очищаем </param>
        internal EngineGL Clear(ClearBufferMask mask)
        {
            GL.Clear(mask);
            return this;
        }

        /// <summary>
        /// Активирование шейдера,после активации шейдера, 
        /// данные будут отправляться с помощью <see cref="SetShaderData{T}(string, T)"/> именно в этот активный шейдер
        /// </summary>
        /// <param name="shader">Сам шейдер</param>
        internal EngineGL UseShader(Shader shader)
        {
            ActiveShader = shader;
            ActiveShader.Use();
            return this;
        }

        /// <summary>
        /// Установка вьюпорта, идет вроде слева снизу система координат,
        /// то есть слева снизу будет координата (0,0)
        /// </summary>
        /// <param name="x">Начальная координата X</param>
        /// <param name="y">Начальная координата Y</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        internal EngineGL Viewport(int x, int y, int width, int height)
        {
            var viewport = new EngineViewport(x, y, width, height);
            if (ActiveViewport != viewport)
            {
                GL.Viewport(x, y, width, height);
                ActiveViewport = viewport;
            }
            return this;
        }

        /// <summary>
        /// Установка вьюпорта, идет вроде слева снизу система координат,
        /// то есть слева снизу будет координата (0,0)
        /// </summary>
        /// <param name="viewport">Вьюпорт</param>
        internal EngineGL Viewport(EngineViewport viewport)
        {
            if (ActiveViewport != viewport)
            {
                GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
                ActiveViewport = viewport;
            }
            return this;
        }

        /// <summary>
        /// Установка режима отрисовки
        /// </summary>
        /// <param name="face">какую сторону отрисовывать</param>
        /// <param name="mode">в каком режиме отрисовывать</param>
        internal EngineGL PolygonMode(MaterialFace face, PolygonMode mode)
        {
            if (activePolygonMode != mode || ActivePolygonFace != face)
            {
                GL.PolygonMode(face, mode);
                ActivePolygonFace = face;
                ActivePolygonMode = mode;
            }
            return this;
        }

        /// <summary>
        /// Активация юнит текстуры
        /// </summary>
        /// <param name="unit">Какой юнит активируем текстуры</param>
        /// <returns></returns>
        internal EngineGL ActiveTexture(TextureUnit unit)
        {
            if (unit != ActiveTextureUnit)
            {
                GL.ActiveTexture(unit);
                ActiveTextureUnit = unit;
            }
            return this;
        }

        /// <summary>
        /// Отправка индекса текстуры
        /// </summary>
        /// <param name="target">тип текстуры</param>
        /// <param name="textureID">индекс текстуры</param>
        internal EngineGL BindTexture(TextureTarget target, int textureID)
        {
            if (textureID != BindedTexID)
            {
                GL.BindTexture(target, textureID);
                BindedTexID = textureID;
                BindedTexture = null;
            }
            return this;
        }

        /// <summary>
        /// Отправка индекса текстуры
        /// </summary>
        /// <param name="target">тип текстуры</param>
        /// <param name="texture">Отправляемая текстура</param>
        internal EngineGL BindTexture(TextureTarget target, ITexture texture)
        {
            if (texture != BindedTexture && texture.Handle != BindedTexID)
            {
                GL.BindTexture(target, texture.Handle);
                BindedTexID = texture.Handle;
                BindedTexture = texture;
            }
            return this;
        }

        /// <summary>
        /// Отправка информации в буффер
        /// </summary>
        /// <typeparam name="T">Тип отправляемой информации в буффер </typeparam>
        /// <param name="target">Тип в какой буффер отправляем,должен быть сначала присоединён с помощью <see cref="BindBuffer(BufferTarget, int)"/></param>
        /// <param name="size">Размер отправляемой информации</param>
        /// <param name="data">Сама информация</param>
        /// <param name="hint">Тип как будет использоваться данные. К примеру,
        /// StaticDraw - данные буфера не будут меняться,
        /// можно использовать другие варианты, в будущем</param>
        internal EngineGL BufferData<T>(BufferTarget target, int size, T[] data, BufferUsageHint hint) where T : struct
        {
            GL.BufferData(target, size, data, hint);
            return this;
        }

        /// <summary>
        /// Отправка информации в буффер
        /// </summary>
        /// <param name="target">Тип в какой буффер отправляем,должен быть сначала присоединён с помощью <see cref="BindBuffer(BufferTarget, int)"/></param>
        /// <param name="size">Размер отправляемой информации</param>
        /// <param name="data">Сама информация</param>
        /// <param name="hint">Тип как будет использоваться данные. К примеру,
        /// StaticDraw - данные буфера не будут меняться,
        /// можно использовать другие варианты, в будущем</param>
        internal EngineGL BufferData(BufferTarget target, int size, IntPtr data, BufferUsageHint hint)
        {
            GL.BufferData(target, size, data, hint);
            return this;
        }

        /// <summary>
        /// Отправка информации в буффер
        /// </summary>
        /// <typeparam name="T">Тип отправляемой информации в буффер </typeparam>
        /// <param name="target">Тип в какой буффер отправляем,должен быть сначала присоединён с помощью <see cref="BindBuffer(BufferTarget, int)"/></param>
        /// <param name="size">Размер отправляемой информации</param>
        /// <param name="data">Сама информация</param>
        /// <param name="hint">Тип как будет использоваться данные. К примеру,
        /// StaticDraw - данные буфера не будут меняться,
        /// можно использовать другие варианты, в будущем</param>
        internal EngineGL BufferData<T>(BufferTarget target, IntPtr size, T[] data, BufferUsageHint hint) where T : struct
        {
            GL.BufferData(target, size, data, hint);
            return this;
        }

        /// <summary>
        /// Присоединение буффера кадра
        /// </summary>
        /// <param name="target">целевой тип буффера кадра</param>
        /// <param name="frameBuffer">Сам буффер кадра</param>
        /// <returns></returns>
        internal EngineGL BindFramebuffer(FramebufferTarget target, FrameBuffer frameBuffer)
        {
            if (frameBuffer != CurrentFrameBuffer)
            {
                GL.BindFramebuffer(target, frameBuffer.Handle);
                CurrentFrameBuffer = frameBuffer;
            }
            return this;
        }

        /// <summary>
        /// Установка параметров текстуры
        /// </summary>
        /// <param name="target">тип целевой текстуры</param>
        /// <param name="paramName">Название параметра</param>
        /// <param name="param">Сам параметр</param>
        internal EngineGL TexParameter(TextureTarget target, TextureParameterName paramName, int param)
        {
            GL.TexParameter(target, paramName, param);
            return this;
        }

        /// <summary>
        /// Установка параметров текстуры
        /// </summary>
        /// <param name="target">тип целевой текстуры</param>
        /// <param name="paramName">Название параметра</param>
        /// <param name="param">Параметры</param>
        internal EngineGL TexParameter(TextureTarget target, TextureParameterName paramName, float[] param)
        {
            GL.TexParameter(target, paramName, param);
            return this;
        }

        /// <summary>
        /// Создание 2D текстуры 
        /// </summary>
        /// <param name="textureTarget">Тип целевой текстуры</param>
        /// <param name="level">видимо слой</param>
        /// <param name="pixelInternalFormat">Формат пикселей</param>
        /// <param name="width">ширина </param>
        /// <param name="height">высота</param>
        /// <param name="border">что-то с границами связано</param>
        /// <param name="pixelFormat">еще формат пикселей</param>
        /// <param name="pixelType">Тип пикселей</param>
        /// <param name="pixels">ссылка на загруженную текстуру</param>
        internal EngineGL TexImage2D(TextureTarget textureTarget, int level, PixelInternalFormat pixelInternalFormat, int width, int height, int border, PixelFormat pixelFormat, PixelType pixelType, IntPtr pixels)
        {
            GL.TexImage2D(textureTarget, level, pixelInternalFormat, width, height, border, pixelFormat, pixelType, pixels);
            return this;
        }

        /// <summary>
        /// Создаем объект текстуры
        /// </summary>
        /// <param name="tex">на выход подается переменная, которая будет хранить индекс текстуры</param>
        /// <returns></returns>
        internal EngineGL GenTexture(out int tex)
        {
             tex = GL.GenTexture();
            return this;
        }

        /// <summary>
        /// Генерируем буффер
        /// </summary>
        /// <param name="vbo">возращается индекс буффера</param>
        internal EngineGL GenBuffer(out int vbo)
        {
            vbo = GL.GenBuffer();
            return this;
        }

        /// <summary>
        /// Генерируем буффер кадра
        /// </summary>
        /// <param name="fbo">возращается индекс буффера када</param>
        internal EngineGL GenFramebuffer(out int fbo)
        {
            fbo = GL.GenFramebuffer();
            return this;
        }

        /// <summary>
        /// Генерация мипмапа,чтобы при отдалении текстура уменьшалась, а при увеличении увеличивалась, сложно объяснить
        /// </summary>
        /// <param name="target"></param>
        internal EngineGL GenerateMipmap(GenerateMipmapTarget target)
        {
            GL.GenerateMipmap(target);
            return this;
        }

        /// <summary>
        /// Установка текстуры буффера кадра 2D
        /// </summary>
        /// <param name="framebufferTarget">на какой буффер накладываем текстуру</param>
        /// <param name="framebufferAttachment">привязка буффера кадра</param>
        /// <param name="textureTarget">целевой тип текстуры</param>
        /// <param name="texture">Текстура</param>
        /// <param name="level">Уровень наложения текстуры</param>
        internal EngineGL FramebufferTexture2D(FramebufferTarget framebufferTarget, FramebufferAttachment framebufferAttachment, TextureTarget textureTarget, ITexture texture, int level)
        {
            GL.FramebufferTexture2D(framebufferTarget, framebufferAttachment, textureTarget, texture.Handle, level);
            return this;
        }

        /// <summary>
        /// Установка текстуры буффера кадра
        /// </summary>
        /// <param name="framebufferTarget">на какой буффер накладываем текстуру</param>
        /// <param name="framebufferAttachment">привязка буффера кадра</param>
        /// <param name="texture">Текстура</param>
        /// <param name="level">Уровень наложения текстуры</param>
        internal EngineGL FramebufferTexture(FramebufferTarget framebufferTarget, FramebufferAttachment framebufferAttachment, ITexture texture, int level)
        {
            GL.FramebufferTexture(framebufferTarget, framebufferAttachment, texture.Handle, level);
            return this;
        }

        internal EngineGL DeleteBuffer(IBufferVerticesObject bo)
        {
            GL.DeleteBuffer(bo.Handle);
            return this;
        }
    }
}
