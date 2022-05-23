using System.Runtime.InteropServices;

using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Base.Buffers
{
    public struct PBO : IBufferObject
    {
        private int _handle;

        public int Handle { get => _handle; set => _handle = value; }

        public void Bind(BufferTarget target)
        {
            throw new NotImplementedException();
        }

        public void Bind() => EngineGL.Instance.BindBuffer(BufferTarget.PixelPackBuffer, this);

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IBufferObject Setup()
        {
            EngineGL.Instance.GenBuffer(out _handle);
            Bind();
            EngineGL.Instance.BufferData(BufferTarget.PixelPackBuffer, Game.Width * Game.Height * sizeof(float), (IntPtr)0, BufferUsageHint.StreamRead);
            return this;
        }
        public IBufferObject Setup(int dataSize, IntPtr scan)
        {
            EngineGL.Instance.GenBuffer(out _handle);
            Bind();
            EngineGL.Instance.BufferData(BufferTarget.PixelUnpackBuffer, dataSize, (IntPtr)null, BufferUsageHint.StreamDraw);
            var ptr = GL.MapBuffer(BufferTarget.PixelUnpackBuffer, BufferAccess.WriteOnly);
            if (ptr != (IntPtr)null)
            {
                unsafe
                {
                    Task.Run(() =>
                    {
                        CopyMemory(scan, ptr, dataSize);
                    });
                }
            }
            if (!GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer))
            {
                throw new Exception("CANT UNMAP BUFFER");
            }
            return this;
        }

        public IBufferObject Process(ITexture tex)
        {
            Bind();
            GL.ReadPixels(0, 0, tex.Size.X, tex.Size.Y, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
            var sync = GL.FenceSync(SyncCondition.SyncGpuCommandsComplete, WaitSyncFlags.None);

            GL.GetSync(sync, SyncParameterName.SyncStatus, sizeof(int), out int lengs, out int result);
            if (result == (int)ArbSync.Signaled)
            {

                tex.Pbos[tex.CurrentPboIndex + 1].Bind();

                var mappedBuffer = GL.MapBuffer(BufferTarget.PixelPackBuffer, BufferAccess.ReadOnly);
                //Texture.Memcpy(tex.Pointer, mappedBuffer, (int)tex.DataSize);
                GL.UnmapBuffer(BufferTarget.PixelPackBuffer);
                Unbind();
                tex.CurrentPboIndex++;
                if (tex.CurrentPboIndex == ITexture.MAX_PBOS - 1)
                {
                    tex.CurrentPboIndex = 0;
                }
            }
            return this;
        }

        public void Unbind(BufferTarget target) => throw new NotImplementedException();

        public void Unbind() => EngineGL.Instance.UnbindBuffer(BufferTarget.PixelPackBuffer);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, int count);

    }
}
