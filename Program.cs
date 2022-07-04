using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PloomesTest.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("secrets.json");

var address = builder.Configuration["SQLServerAddress"];
var initialCatalog = builder.Configuration["SQLServerInitialCatalog"];
var userId = builder.Configuration["SQLServerUserId"];
var password = builder.Configuration["SQLServerPassword"];
var persistSecurityInfo = builder.Configuration["PersistSecurityInfo"];
var multipleActiveResultSets = builder.Configuration["MultipleActiveResultSets"];
var encrypt = builder.Configuration["Encrypt"];
var trustServerCertificate = builder.Configuration["TrustServerCertificate"];
var connectionTimeout = builder.Configuration["ConnectionTimeout"];

var connectionString = builder.Configuration.GetConnectionString("Customers") ?? $"Server={address}; Initial Catalog={initialCatalog}; User Id={userId}; Password={password};MultipleActiveResultSets={multipleActiveResultSets};Encrypt={encrypt};TrustServerCertificate={trustServerCertificate};Connection Timeout={connectionTimeout}";


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(c =>
{
    c.AddPolicy("AnyOrigin", builder =>
    {
        builder
            .AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddSqlServer<DataContext>(connectionString);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "Ploomes API", Description = "Written by me, João Victor. I wrote it with C# and integrated it to SQL Server. Reach me on github: https://github.com/elcanion", Version = "v2"}); 
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AnyOrigin");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Ploomes API V2");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();
