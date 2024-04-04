using AspNetCoreRateLimit;
using CompanyEmployees.Presentation.ActionFilters;
using Contracts;
using Entities.Models;
using MelvinBankApi.Extensions;
using MelvinBankApi.Presentation.ActionFilters;
using MelvinBankApi.Utility;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Serilog;
using Service;
using Service.Contracts;
using Service.DataShaping;
using Shared.DataTranferObjects;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers(config =>
{
	config.RespectBrowserAcceptHeader = true;
	config.ReturnHttpNotAcceptable = true;
	//config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
	config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
}).AddXmlDataContractSerializerFormatters()
  .AddCustomCSVFormatter()
  .AddApplicationPart(typeof(MelvinBankApi.Presentation.AssemblyReference).Assembly);

builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateMediaTypeAttribute>();
builder.Services.AddScoped<IDataShaper<GetAccountDto>, DataShaper<GetAccountDto>>();
builder.Services.AddScoped<IAccountLinks, AccountLinks>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureCors();
builder.Services.ConfigurIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureAuthenticateAccountNumberService();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureAppSettings(builder.Configuration);
builder.Services.AddCustomMediaTypes();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);



var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
	app.UseHsts();	
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.All
});

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseIpRateLimiting();

app.UseCors("AllowAll");

app.UseResponseCaching();

app.UseHttpCacheHeaders();

app.UseAuthentication();

app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(s =>
{
	//var prefix = string.IsNullOrEmpty(s.RoutePrefix) ? "." : "..";
	//s.SwaggerEndpoint($"{prefix}/swgger/v1/swagger.json", "MelvinBank API Documentation");
	s.SwaggerEndpoint("/swagger/v1/swagger.json", "MelvinBank API v1");
	s.SwaggerEndpoint("/swagger/v2/swagger.json", "MelvinBank API v2");

});

app.MapControllers();

app.Run();
