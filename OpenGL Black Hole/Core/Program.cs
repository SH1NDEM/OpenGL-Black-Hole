using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using System.Numerics;

var opts   = WindowOptions.Default;
opts.Size  = new Vector2D<int>(800, 600);
opts.Title = "OpenGL Cube";
var window = Window.Create(opts);

GL   gl   = null!;
uint vao  = 0;
uint vbo  = 0;
uint ebo  = 0;
uint prog = 0;
float t   = 0f;

// ── Геометрия ─────────────────────────────────────────────────────────────────
// 24 вершины (4 на грань × 6 граней). Каждая вершина: pos(xyz) + normal(xyz).
// У каждой грани своя нормаль — нельзя делить вершины между гранями,
// иначе нормаль будет усреднённой и flat-shading не получится.
float[] verts =
{
    // Формат: posX posY posZ   normX normY normZ

    // Передняя грань (+Z), нормаль смотрит вперёд
    -0.5f,-0.5f, 0.5f,  0, 0, 1,
     0.5f,-0.5f, 0.5f,  0, 0, 1,
     0.5f, 0.5f, 0.5f,  0, 0, 1,
    -0.5f, 0.5f, 0.5f,  0, 0, 1,

    // Задняя грань (-Z)
     0.5f,-0.5f,-0.5f,  0, 0,-1,
    -0.5f,-0.5f,-0.5f,  0, 0,-1,
    -0.5f, 0.5f,-0.5f,  0, 0,-1,
     0.5f, 0.5f,-0.5f,  0, 0,-1,

    // Правая грань (+X)
     0.5f,-0.5f, 0.5f,  1, 0, 0,
     0.5f,-0.5f,-0.5f,  1, 0, 0,
     0.5f, 0.5f,-0.5f,  1, 0, 0,
     0.5f, 0.5f, 0.5f,  1, 0, 0,

    // Левая грань (-X)
    -0.5f,-0.5f,-0.5f, -1, 0, 0,
    -0.5f,-0.5f, 0.5f, -1, 0, 0,
    -0.5f, 0.5f, 0.5f, -1, 0, 0,
    -0.5f, 0.5f,-0.5f, -1, 0, 0,

    // Верхняя грань (+Y)
    -0.5f, 0.5f, 0.5f,  0, 1, 0,
     0.5f, 0.5f, 0.5f,  0, 1, 0,
     0.5f, 0.5f,-0.5f,  0, 1, 0,
    -0.5f, 0.5f,-0.5f,  0, 1, 0,

    // Нижняя грань (-Y)
    -0.5f,-0.5f,-0.5f,  0,-1, 0,
     0.5f,-0.5f,-0.5f,  0,-1, 0,
     0.5f,-0.5f, 0.5f,  0,-1, 0,
    -0.5f,-0.5f, 0.5f,  0,-1, 0,
};

// 2 треугольника на грань, 6 граней = 36 индексов
uint[] idx =
{
     0, 2, 1,  2, 0, 3,
     4, 6, 5,  6, 4, 7,
     8,10, 9, 10, 8,11,
    12,14,13, 14,12,15,
    16,18,17, 18,16,19,
    20,22,21, 22,20,23,
};


// ── Шейдеры ───────────────────────────────────────────────────────────────────
// ── (Спизжены, но подкорректированны) ─────────────────────────────────────────
const string VS = @"#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;

uniform mat4 uMVP;    // Model-View-Projection
uniform mat4 uModel;  // только Model — для поворота нормалей в world-space

out vec3 vNormal;

void main()
{
    gl_Position = uMVP * vec4(aPos, 1.5);

    // Нормаль нужно повернуть вместе с объектом (берём 3x3 из матрицы модели)
    vNormal = mat3(uModel) * aNormal;
}";

const string FS = @"#version 330 core
in  vec3 vNormal;
out vec4 FragColor;

void main()
{
    // Направление источника света в world-space (сверху-спереди-справа)
    vec3 lightDir = normalize(vec3(1.5, 0.7, -0.2));

    // Lambert: насколько грань смотрит на свет
    float diffuse = max(dot(normalize(vNormal), lightDir), 0.1);

    // Итоговая яркость = фоновый свет + диффузный
    float brightness = 0.15 + 0.85 * diffuse;

    FragColor = vec4(vec3(brightness), 0.5);  // белый куб с тенями
}";

// ── Запуск ────────────────────────────────────────────────────────────────────
window.Load   += Load;
window.Render += Render;
window.Run();

// ─────────────────────────────────────────────────────────────────────────────
void Load()
{
    gl = GL.GetApi(window);
    gl.ClearColor(0.15f, 0.15f, 0.15f, 1f);
    gl.Enable(EnableCap.DepthTest);   // ближние грани перекрывают дальние
    gl.Enable(EnableCap.CullFace);    // не рисуем обратные грани (оптимизация)


    var fb = window.FramebufferSize;
    gl.Viewport(0, 0, (uint)fb.X, (uint)fb.Y);

    unsafe
    {
        vao = gl.GenVertexArray();
        gl.BindVertexArray(vao);

        // VBO - вершины (pos + normal)
        vbo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
        fixed (float* p = verts)
            gl.BufferData(BufferTargetARB.ArrayBuffer,
                (nuint)(verts.Length * sizeof(float)), p, BufferUsageARB.StaticDraw);

        // EBO — индексы
        ebo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
        fixed (uint* p = idx)
            gl.BufferData(BufferTargetARB.ElementArrayBuffer,
                (nuint)(idx.Length * sizeof(uint)), p, BufferUsageARB.StaticDraw);

        // Атрибут 0: позиция  (3 float, шаг 24 байта, смещение 0)
        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 24, (void*)0);
        gl.EnableVertexAttribArray(0);

        // Атрибут 1: нормаль  (3 float, шаг 24 байта, смещение 12)
        gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 24, (void*)12);
        gl.EnableVertexAttribArray(1);

        gl.BindVertexArray(0);
    }

    // Компиляция шейдеров
    uint vs = Compile(ShaderType.VertexShader,   VS);
    uint fs = Compile(ShaderType.FragmentShader, FS);

    prog = gl.CreateProgram();
    gl.AttachShader(prog, vs);
    gl.AttachShader(prog, fs);
    gl.LinkProgram(prog);

    gl.GetProgram(prog, ProgramPropertyARB.LinkStatus, out int ok);
    if (ok == 0) Console.Error.WriteLine("Ошибка линковки: " + gl.GetProgramInfoLog(prog));

    gl.DeleteShader(vs);
    gl.DeleteShader(fs);
}

// ─────────────────────────────────────────────────────────────────────────────
void Render(double delta)
{
    t += (float)delta;

    gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    var model = Matrix4x4.CreateRotationY(t) * Matrix4x4.CreateRotationX(t * 0.4f);
    var view  = Matrix4x4.CreateLookAt(new Vector3(0f, 1.5f, 3f), Vector3.Zero, Vector3.UnitY);
    var proj  = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4f, 800f / 600f, 0.1f, 100f);

    // System.Numerics хранит строчно (row-major).
    // OpenGL ждёт столбцово (col-major).
    // transpose=true говорит OpenGL: «данные row-major — сам транспонируй».
    // Никакого ручного Transpose() не нужно.
    var mvp = view * proj * model;



    gl.UseProgram(prog);
    unsafe
    {
        int locMVP   = gl.GetUniformLocation(prog, "uMVP");
        int locModel = gl.GetUniformLocation(prog, "uModel");
        gl.UniformMatrix4(locMVP,   1, true, (float*)&mvp);    // transpose=true
        gl.UniformMatrix4(locModel, 1, true, (float*)&model);  // transpose=true
    }

    gl.BindVertexArray(vao);
    unsafe
    {
        gl.DrawElements(PrimitiveType.Triangles, (uint)idx.Length,
            DrawElementsType.UnsignedInt, (void*)0);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
uint Compile(ShaderType type, string src)
{
    uint s = gl.CreateShader(type);
    gl.ShaderSource(s, src);
    gl.CompileShader(s);
    gl.GetShader(s, ShaderParameterName.CompileStatus, out int ok);
    if (ok == 0) Console.Error.WriteLine($"Шейдер ({type}): " + gl.GetShaderInfoLog(s));
    return s;
}
