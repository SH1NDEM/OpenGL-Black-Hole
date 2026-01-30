using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace OpenGL_Black_Hole.Render;

public class Renderer
{
    private GL gl;

    public void Initialize(IWindow window)
    {
        gl = GL.GetApi(window);

        // Серый фон
        gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);
    }

    public void Render(double delta)
    {
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
    }
}