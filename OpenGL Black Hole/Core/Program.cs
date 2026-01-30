using Silk.NET.Windowing;
using Silk.NET.Maths;
using OpenGL_Black_Hole.Render;

namespace OpenGL_Black_Hole.Core;

public class Program
{
    private static IWindow window;
    private static Renderer renderer;

    static void Main(string[] args)
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "OpenGL Black Hole";

        window = Window.Create(options);

        renderer = new Renderer();

        window.Load += OnLoad;
        window.Render += OnRender;

        window.Run();
    }

    private static void OnLoad()
    {
        renderer.Initialize(window);
    }

    private static void OnRender(double delta)
    {
        renderer.Render(delta);
    }
}