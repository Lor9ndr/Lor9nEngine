using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine
{
    public static class Logger
    {
        public static DebugProc DebugProcCallback = DebugCallback;
        public static GCHandle DebugProcCallbackHandle;
        private static void DebugCallback(DebugSource source, DebugType type, int id,
     DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);
            if (severity == DebugSeverity.DebugSeverityNotification)
            {
                return;
            }
            Console.WriteLine($"{severity} {type} | {messageString}");

            if (type == DebugType.DebugTypeError)
                throw new Exception(messageString);
        }
        public static void ClearError()
        {
            while (GL.GetError() != ErrorCode.NoError)
            {

            }
        }
    }
}
