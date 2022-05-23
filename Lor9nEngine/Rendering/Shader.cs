using NLog;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Lor9nEngine.Rendering
{
    /// <summary>
    /// Класс шейдера
    /// </summary>
    public class Shader : IDisposable
    {
        /// <summary>
        /// Индекс шейдера
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// Возможные значения, которые нужно заполнить в шейдере
        /// </summary>
        public readonly Dictionary<string, int> UniformLocations;
        private readonly string _path;
        private static int _ActiveHandle;
        private bool disposedValue = false;
        public string Path => _path;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Конструктор для получения шейдера с геометрическим шейдером
        /// </summary>
        /// <param name="vertPath">Путь к вершинному шейдеру</param>
        /// <param name="fragPath">Путь к фрагментному шейдеру</param>
        /// <param name="geomPath">Путь к геометрическому шейдеру</param>
        public Shader(string vertPath, string fragPath, string geomPath)
        {

            _path = vertPath;
            // Load vertex shader and compile
            var shaderSource = File.ReadAllText(vertPath);

            // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);


            // Now, bind the GLSL source code
            GL.ShaderSource(vertexShader, shaderSource);

            // And then compile
            CompileShader(vertexShader);

            // We do the same for the fragment shader
            shaderSource = File.ReadAllText(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);

            shaderSource = File.ReadAllText(geomPath);
            int geometryShader = GL.CreateShader(ShaderType.GeometryShader);
            GL.ShaderSource(geometryShader, shaderSource);
            CompileShader(geometryShader);


            // These 3 shaders must then be merged into a shader program, which can then be used by OpenGL.
            // To do this, create a program...
            ID = GL.CreateProgram();

            // Attach  shaders...
            GL.AttachShader(ID, vertexShader);
            GL.AttachShader(ID, fragmentShader);

            GL.AttachShader(ID, geometryShader);
            // And then link them together.
            LinkProgram(ID);

            // When the shader program is linked, it no longer needs the individual shaders attacked to it; the compiled code is copied into the shader program.
            // Detach them, and then delete them.
            GL.DetachShader(ID, vertexShader);
            GL.DetachShader(ID, fragmentShader);
            GL.DetachShader(ID, geometryShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(geometryShader);

            // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
            // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
            // later.

            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(ID, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Next, allocate the dictionary to hold the locations.
            UniformLocations = new Dictionary<string, int>();

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(ID, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(ID, key);

                // and then add it to the dictionary.
                UniformLocations.Add(key, location);
            }
        }

        /// <summary>
        /// Конструктор для получения шейдера только с вершинным и фрагментным шейдерами
        /// </summary>
        /// <param name="vertPath">Путь к вершинному шейдеру</param>
        /// <param name="fragPath">Путь к фрагментному шейдеру</param>
        public Shader(string vertPath, string fragPath)
        {
            _path = vertPath;
            // Load vertex shader and compile
            var shaderSource = File.ReadAllText(vertPath);

            // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);


            // Now, bind the GLSL source code
            GL.ShaderSource(vertexShader, shaderSource);
            shaderSource = File.ReadAllText(fragPath);
            // And then compile
            CompileShader(vertexShader);

            // We do the same for the fragment shader
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);
            ID = GL.CreateProgram();


            // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
            // To do this, create a program...

            // Attach both shaders...
            GL.AttachShader(ID, vertexShader);
            GL.AttachShader(ID, fragmentShader);

            // And then link them together.
            LinkProgram(ID);

            // When the shader program is linked, it no longer needs the individual shaders attacked to it; the compiled code is copied into the shader program.
            // Detach them, and then delete them.
            GL.DetachShader(ID, vertexShader);
            GL.DetachShader(ID, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
            // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
            // later.

            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(ID, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Next, allocate the dictionary to hold the locations.
            UniformLocations = new Dictionary<string, int>();

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(ID, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(ID, key);

                // and then add it to the dictionary.
                UniformLocations.Add(key, location);
            }
        }

        /// <summary>
        /// Компиляция шейддера
        /// </summary>
        /// <param name="shader"> индекс шейдера</param>
        /// <exception cref="Exception">Если ошибка компиляции выдает ошибку</exception>
        private static void CompileShader(int shader)
        {
            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        /// <summary>
        /// Присоединение разных шейдеров в одну программу
        /// </summary>
        /// <param name="program">индекс программы</param>
        /// <exception cref="Exception">Выдает ошибку, если при соединении вышла ошибка, например, когда шейдеры имеют разные in - out переменные </exception>
        private void LinkProgram(int program)
        {
            // We link the program
            GL.LinkProgram(program);

            // Check for linking errors
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                Console.WriteLine(GL.GetProgramInfoLog(program));
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"{GL.GetProgramInfoLog(program)}, path- {_path})");
            }
        }

        /// <summary>
        /// Активация шейдера
        /// </summary>
        public void Use()
        {
            GL.UseProgram(ID);
            _ActiveHandle = ID;
        }

        /// <summary>
        /// Получение индекс расположения по названию переменной
        /// Например, в шейдере можно указать строго заданные места переменных, с помощью location(x)... где x - индекс расположения переменной
        /// </summary>
        /// <param name="attribName">название переменной</param>
        /// <returns></returns>

        public int GetAttribLocation(string attribName) => GL.GetAttribLocation(ID, attribName);

        /// <summary>
        /// Отправка в шейдер целочисленную переменную 
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public void SetInt(string name, int data) => GL.Uniform1(GetUniformLocation(name), data);

        /// <summary>
        /// Отправка в шейдер булевую переменную
        /// в GLSL нельзя вроде отправить булевую переменную поэтому перевод в целочисленный тип,
        /// где 1 == true, а 0 == false
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public void SetInt(string name, bool data) => GL.Uniform1(GetUniformLocation(name), data ? 1 : 0);

        /// <summary>
        /// Отправка в шейдер плавающее число
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public void SetFloat(string name, float data) => GL.Uniform1(GetUniformLocation(name), data);

        /// <summary>
        /// Отправка в шейдер матрицу 4x4
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public void SetMatrix4(string name, Matrix4 data) => GL.UniformMatrix4(GetUniformLocation(name), false, ref data);

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public void SetVector3(string name, Vector3 data) => GL.Uniform3(GetUniformLocation(name), data);

        /// <summary>
        /// Отправка в шейдер 2D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public void SetVector2(string name, Vector2 data) => GL.Uniform2(GetUniformLocation(name), data);

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="x">координата X</param>
        /// <param name="y">координата Y</param>
        /// <param name="z">координата Z</param>
        public void SetVector3(string name, float x, float y, float z) => SetVector3(name, new Vector3(x, y, z));

        /// <summary>
        /// Получение расположение переменной в шейдере,
        /// Если нам уже известно расположение,то возвращаем,
        /// иначе ищем с помощью <see cref="GL"/>
        /// </summary>
        /// <param name="name">Название переменной</param>
        private int GetUniformLocation(string name)
        {
            if (UniformLocations.ContainsKey(name))
            {
                return UniformLocations[name];
            }
            else
            {
                int pos = GL.GetUniformLocation(ID, name);
                UniformLocations.Add(name, pos);

                if (pos == -1)
                {
                    var path = _path.Split('/').Where(s => s.Contains('.')).ToList().Last();

                    var error = $"{name} Was not setted in {path}";
                    Console.WriteLine(error);
                    _logger.Warn(error);
                }
                return pos;
            }
        }

        /// <summary>
        /// Удаление шейдера из памяти и тд
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Удаление шейдера из памяти и тд
        /// </summary>
        ~Shader()
        {
        }
        /// <summary>
        /// Удаление шейдера из памяти и тд
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(ID);

                disposedValue = true;
            }
        }
    }
}