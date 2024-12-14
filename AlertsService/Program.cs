using AlertsService.Models;
using Microsoft.Extensions.Options;
using AlertsService;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Укажите ваш фронтенд-URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
// Добавляем EmailSettings из конфигурации
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Регистрируем сервис отправки почты
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// Настроить конвейер обработки HTTP-запросов
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");
app.MapControllers();
app.Run();