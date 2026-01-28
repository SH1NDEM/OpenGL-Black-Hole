using System;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;

class Program
{
    static IWindow window;
    static GL gl;

    static void Main(string[] args)
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "Silk.NET OpenGL";

        window = Window.Create(options);

        window.Load += OnLoad;
        window.Render += OnRender;

        window.Run();
    }

    static void OnLoad()
    {
        gl = GL.GetApi(window);
        gl.ClearColor(0.1f, 0.1f, 0.15f, 1f);
    }

    static void OnRender(double delta)
    {
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
    }
}
