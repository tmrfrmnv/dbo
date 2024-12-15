using AlertsService.Models;
using Microsoft.Extensions.Options;
using AlertsService;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������� � ���������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // ������� ��� ��������-URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    // ���������� �������� ��� Swagger
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Alerts Service API",
        Version = "v1",
        Description = "�������� ����������� �� �������� ��������� � ���������",
        Contact = new OpenApiContact
        {
            Name = "��� GitHub",
            Url = new Uri("https://github.com/tmrfrmnv")
        },
    });
});
// ��������� EmailSettings �� ������������
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// ������������ ������ �������� �����
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// ��������� �������� ��������� HTTP-��������
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");
app.MapControllers();
app.Run();