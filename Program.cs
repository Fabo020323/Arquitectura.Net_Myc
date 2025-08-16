using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Minio;

using ProyectTemplate.Services.MinioService;
using ProyectTemplate.Utils.Options;

using SuntravelDbContext = ProyectTemplate.Data.ProjectTemplateDbContext;

var builder = WebApplication.CreateBuilder(args);

//Conection to db
builder.Services.AddDbContext<SuntravelDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("stringConnection")));
//Connect to minio
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection(MinioOptions.SectionName));
builder.Services.AddScoped<IBlobServices, MinioBlobServices>();
// Add services to the container.
builder.Services
    .AddAuthorization()
    .AddFastEndpoints()
    .AddSwaggerGen(); ;


builder.Services.AddAuthentication();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Back Net";
        s.Version = "v1";

    };
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.UseSwaggerGen();

app.UseFastEndpoints(c => c.Endpoints.RoutePrefix = "backnet");

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SuntravelDbContext>();
    dbContext.Database.Migrate();

    var minioOptions = scope.ServiceProvider.GetRequiredService<IOptions<MinioOptions>>().Value;

    var minioClient = new MinioClient()
      .WithEndpoint(minioOptions.Endpoint, minioOptions.Port)
      .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
      .WithSSL(true)
      .Build();
}

app.Run();