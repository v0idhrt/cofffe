var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();

// Настраиваем CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin() // Разрешает запросы с любых доменов
              .AllowAnyMethod() // Разрешает любые HTTP-методы (GET, POST, OPTIONS и т.д.)
              .AllowAnyHeader(); // Разрешает любые заголовки
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

// Включаем CORS
app.UseCors();

app.MapControllers(); // Настраиваем маршруты для контроллеров

app.Run();