using Silk.NET.Windowing;
using Silk.NET.Maths;
using OpenGL_Black_Hole.Render;

namespace OpenGL_Black_Hole.Core;

public class Program
{
    // Два поля, которые хранят состояние приложения 
    private static IWindow window;
    private static Renderer renderer;

    static void Main(string[] args)
    {
        var options = WindowOptions.Default;        // Старые настройки окна
        options.Size = new Vector2D<int>(800, 600); // Устанавливаем размер
        options.Title = "OpenGL Black Hole";        // Заголовок окна

        window = Window.Create(options);            // Окно на основе настроек

        renderer = new Renderer();                  // Объект рендера (OpenGL и отрисовка)

        window.Load += OnLoad;                      // Подписываемся на загрузку окна (событие один раз)
        window.Render += OnRender;                  // Подписываемся на рендер (событие каждый кадр)

        window.Run();                               // Запуск главного цикла
    }

    /// <summary>
    /// Инициализация OpenGL и ресурсов рендера
    /// </summary>
    private static void OnLoad()
    {
        renderer.Initialize(window);
    }

    /// <summary>
    /// Метод вызывается каждый кадр, 
    /// </summary>
    /// <param name="delta">время, прошедшее с предыдущего кадра</param>
    private static void OnRender(double delta)
    {

        renderer.Render(delta);                     // Отрисовка кадров в поле window
    }
}