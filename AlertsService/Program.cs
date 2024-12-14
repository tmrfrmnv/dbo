using AlertsService.Models;
using Microsoft.Extensions.Options;
using AlertsService;

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
// ��������� EmailSettings �� ������������
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// ������������ ������ �������� �����
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// ��������� �������� ��������� HTTP-��������
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");
app.MapControllers();
app.Run();