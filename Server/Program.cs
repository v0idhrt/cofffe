using Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

// ��������� �����������
builder.Services.AddControllers();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7120") });
// ����������� CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin() // ��������� ������� � ����� �������
              .AllowAnyMethod() // ��������� ����� HTTP-������ (GET, POST, OPTIONS � �.�.)
              .AllowAnyHeader(); // ��������� ����� ���������
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

// �������� CORS
app.UseCors();

app.MapControllers(); // ����������� �������� ��� ������������

app.Run();