using AIcontrolComputer.Models.AIAnswerProcessing;
using Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

// ��������� �����������
builder.Services.AddControllers();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7120") });
builder.Services.AddScoped<IResponseProcessingModule, ResponseProcessingModule>();
builder.Services.AddTransient<ConsultantNewsController>();
builder.Services.AddTransient<RgBusinessController>();


// ��������� �����������
builder.Services.AddControllers();


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