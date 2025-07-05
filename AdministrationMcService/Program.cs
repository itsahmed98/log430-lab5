using AdministrationMcService.Data;
using AdministrationMcService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddScoped<IRapportService, RapportService>();
builder.Services.AddScoped<IMagasinService, MagasinService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient("CatalogueMcService", client =>
{
    client.BaseAddress = new Uri(config["Services:Catalogue"]);
});

builder.Services.AddHttpClient("InventaireMcService", client =>
{
    client.BaseAddress = new Uri(config["Services:Inventaire"]);
});

builder.Services.AddHttpClient("VenteMcService", client =>
{
    client.BaseAddress = new Uri(config["Services:Vente"]);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Administration Service API",
        Version = "v1",
        Description = "Microservice pour la gestion administrative des magasins tels que générer des rapports consolidé des ventes et visualiser les performances."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Administration Service API V1");
    c.RoutePrefix = "swagger";
});

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();

public partial class Program { }
