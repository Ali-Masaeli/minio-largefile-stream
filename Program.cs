using Minio;

var builder = WebApplication.CreateBuilder(args);

var minioClient = new MinioClient()
    .WithEndpoint(new Uri("http://127.0.0.1:9000"))
    .WithCredentials("admin", "12345678")
    .WithSSL(false)
    .Build();

builder.Services.AddSingleton<IMinioClient>(minioClient);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
