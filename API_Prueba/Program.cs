var builder = WebApplication.CreateBuilder(args);

// Agrega Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddControllers(); 

var app = builder.Build();

// Habilita Swagger siempre (desarrollo, staging y producción)
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Consumidora v1");
        c.RoutePrefix = "swagger"; // Puedes acceder en /swagger
    });
}

// Escucha el puerto que da Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
