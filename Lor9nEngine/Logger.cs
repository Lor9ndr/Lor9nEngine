using System.Runtime.InteropServices;

using NLog;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine
{
    public static class Logger
    {
        public static DebugProc DebugProcCallback = DebugCallback;
        public static GCHandle DebugProcCallbackHandle;
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        private static void DebugCallback(DebugSource source, DebugType type, int id,
            DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);
            if (severity == DebugSeverity.DebugSeverityNotification)
            {
                _logger.Debug(messageString);
                return;
            }
            _logger.Warn($"{severity} {type} | {messageString}");

            if (type == DebugType.DebugTypeError)
            {
                var exc = new Exception(messageString);
                _logger.Log(LogLevel.Error, exc);
                throw exc;
            }
        }
        public static void ClearError()
        {
            while (GL.GetError() != ErrorCode.NoError)
            {

            }
        }
    }
}
