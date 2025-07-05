using Microsoft.OpenApi.Models;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

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

builder.Services.AddHttpClient("AdministrationMcService", client =>
{
    client.BaseAddress = new Uri(config["Services:Administration"]);
});

builder.Services.AddHttpClient("ECommerceMcService", client =>
{
    client.BaseAddress = new Uri(config["Services:ECommerce"]);
});


builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Magasin Central API",
        Version = "v1",
        Description = "API pour la gestion central des magasins."
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5252")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Magasin Central API V1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseCors("AllowFrontend");

app.UseHttpMetrics(); // Middleware pour les m�triques HTTP
app.MapMetrics();
app.UseResponseCaching();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
