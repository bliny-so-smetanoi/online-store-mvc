using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Mediator;
using Application.UseCases.TodoItem.Commands.Create;
using Infrastructure;
using KeycloakExtension;
using OnlineStore.Filters;
using OnlineStore.Middlewares;
using Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.Host
    .UseSerilog((ctx, loggerCfg) =>
    loggerCfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ApplicationExceptionFilter>();
}).AddJsonOptions(options => {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}).AddViewLocalization()
.AddDataAnnotationsLocalization();

builder.Services.AddKeycloak(builder.Configuration);
builder.Services.AddPersistance(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.ConfigureMediator(typeof(CreateTodoItemCommand));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<SerilogUserEnricherMiddleware>();
app.UseMiddleware<LocalizationMiddleware>();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();