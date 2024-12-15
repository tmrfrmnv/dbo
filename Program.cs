using AlertsService.Models;
using Microsoft.Extensions.Options;
using AlertsService;
using Microsoft.OpenApi.Models;

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
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    // Добавление описания для Swagger
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Alerts Service API",
        Version = "v1",
        Description = "Отправка уведомлений об основных действиях с аккаунтом",
        Contact = new OpenApiContact
        {
            Name = "Мой GitHub",
            Url = new Uri("https://github.com/tmrfrmnv")
        },
    });
});
// Добавляем EmailSettings из конфигурации
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Регистрируем сервис отправки почты
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Настроить конвейер обработки HTTP-запросов
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");
app.MapControllers();
app.Run();