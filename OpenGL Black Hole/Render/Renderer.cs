using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace OpenGL_Black_Hole.Render;

public class Renderer
{
    
    private GL gl;      // GL - объект, который содержит все функции OpenGL для текущего контекста

    /// <summary>
    /// Метод инициализации Renderer
    /// </summary>
    /// <param name="window">интерфейс из Silk.NET.Windowing, который описывает окно приложения</param>
    public void Initialize(IWindow window)
    {
        gl = GL.GetApi(window);     // Получаем OpenGL API для конкретного окна
        
        gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);        // Серый фон
    }
    
    /// <summary>
    /// Очистка кадра и цветового буфера
    /// </summary>
    /// <param name="delta"></param>
    public void Render(double delta)
    {
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);     // Очищаем экран перед рисованием нового кадра
    }
}