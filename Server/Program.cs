var builder = WebApplication.CreateBuilder(args);

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