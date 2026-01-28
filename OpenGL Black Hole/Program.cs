using System;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;

class Program
{
    static IWindow window;
    static GL gl;

    /// <summary>
    /// Запуск программы
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "Graphics OpenGL";

        window = Window.Create(options);

        window.Load += OnLoad;
        window.Render += OnRender;

        window.Run();
    }

    /// <summary>
    /// Загрузка окна OpenGL
    /// </summary>
    static void OnLoad()
    {
        gl = GL.GetApi(window);

        //Серый цвет заднего фона
        gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);
    }


    static void OnRender(double delta)
    {
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
    }
}
