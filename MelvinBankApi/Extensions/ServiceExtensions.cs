﻿using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service.Contracts;
using Service;
using Microsoft.OpenApi.Models;
using Entities.ConfigurationModels;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using MelvinBankApi.Presentation;
using Marvin.Cache.Headers;
using AspNetCoreRateLimit;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MelvinBankApi.Extensions;

public static class ServiceExtensions
{
	public static void ConfigureCors(this IServiceCollection services) =>
	   services.AddCors(options =>
	    {
			options.AddPolicy("AllowAll", builder =>
			builder.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader()
			.WithExposedHeaders("X-Pagination"));
	    });

	public static void ConfigurIISIntegration(this IServiceCollection services) =>
		services.Configure<IISOptions>(options =>
		{

		});

	public static void ConfigureLoggerService(this IServiceCollection service) => service.AddSingleton<ILoggerManager, LoggerManager>();

	public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
		services.AddDbContext<MelvinBankContext>(opts =>
			opts.UseSqlServer(configuration.GetConnectionString("SQLConnectionString")));

	public static void ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration) =>
		services.Configure<Appsettings>(configuration.GetSection("AppSettings"));

	public static void ConfigureRepositoryManager(this IServiceCollection services) =>
		services.AddScoped<IRepositoryManager, RepositoryManager>();

	public static void ConfigureServiceManager(this IServiceCollection services) =>
		services.AddScoped<IServiceManager, ServiceManager>();

	public static void ConfigureAuthenticateAccountNumberService(this IServiceCollection services) =>
		services.AddScoped<IAuthenticateAccountNumber, AuthenticateAccountNumber>();
	public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) => builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));
	public static void ConfigureResponseCaching(this IServiceCollection services) => services.AddResponseCaching();

	public static void ConfigureHttpCacheHeaders(this IServiceCollection services) => 
	  services.AddHttpCacheHeaders        
		((expirationOpt) =>
		{
			expirationOpt.MaxAge = 65;
			expirationOpt.CacheLocation = CacheLocation.Private;
		},
		(validationOpt) =>
		{
			validationOpt.MustRevalidate = true;
		});

	public static void ConfigureRateLimitingOptions(this IServiceCollection services)
	{
		var rateLimitRules = new List<RateLimitRule>
		{

			new RateLimitRule
			{
				Endpoint = "*",
				Limit = 30,
				Period = "5m"
			}
		};

		services.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; });
		services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
		services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
		services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
		services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
	}

	public static void AddCustomMediaTypes(this IServiceCollection services)
	{
		services.Configure<MvcOptions>(config =>
		{
			var systemTextJsonOutputFormatter = config.OutputFormatters
					.OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

			if (systemTextJsonOutputFormatter != null)
			{
				systemTextJsonOutputFormatter.SupportedMediaTypes
				.Add("application/vnd.melvinbank.hateoas+json");
				 systemTextJsonOutputFormatter.SupportedMediaTypes
			    .Add("application/vnd.melvinbank.apiroot+json");
			}

			var xmlOutputFormatter = config.OutputFormatters
					.OfType<XmlDataContractSerializerOutputFormatter>()?
					.FirstOrDefault();

			if (xmlOutputFormatter != null)
			{
				xmlOutputFormatter.SupportedMediaTypes
				.Add("application/vnd.melvinbank.hateoas+xml");
				xmlOutputFormatter.SupportedMediaTypes
			.Add("application/vnd.melvinbank.apiroot+xml");
			}
		});
	}

	public static void ConfigureVersioning(this IServiceCollection services)
	{
		services.AddApiVersioning(opt =>
		{
			opt.ReportApiVersions = true;
			opt.AssumeDefaultVersionWhenUnspecified = true;
			opt.DefaultApiVersion = new ApiVersion(1, 0);
			opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
			opt.Conventions.Controller<AccountsController>()
		   .HasApiVersion(new ApiVersion(1, 0));
			opt.Conventions.Controller<AccountsV2Controller>()
				.HasDeprecatedApiVersion(new ApiVersion(2, 0));
		});
	}

	public static void ConfigureIdentity(this IServiceCollection services)
	{
		var builder = services.AddIdentity<User, IdentityRole>(o =>
		{
			o.Password.RequireDigit = true;
			o.Password.RequireLowercase = false;
			o.Password.RequireUppercase = false;
			o.Password.RequireNonAlphanumeric = false;
			o.Password.RequiredLength = 10;
			o.User.RequireUniqueEmail = true;
		})
		.AddEntityFrameworkStores<MelvinBankContext>()
		.AddDefaultTokenProviders();
	}

	public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
	{
		var jwtConfiguration = new JwtConfiguration();
		configuration.Bind(jwtConfiguration.Section, jwtConfiguration);
		var secretKey = Environment.GetEnvironmentVariable("SECRET");

		services.AddAuthentication(opt =>
		{
			opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,

				ValidIssuer = jwtConfiguration.ValidIssuer,
				ValidAudience = jwtConfiguration.ValidAudience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
			};
		});
	}

	public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration) =>
		services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));


	public static void ConfigureSwagger(this IServiceCollection services)
	{
		services.AddSwaggerGen(s =>
		{
			s.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "MelvinBankApi Doc",
				Version = "v1",	
				Description = "MelvinBank Api Documentation",
				TermsOfService = new Uri("https://example.com/terms"),
				Contact = new OpenApiContact
				{
					Name = "Melvin Technical Service",
					Email = "melvin_tech@gmail.com",
					Url = new Uri("https://twitter.com/melvin_tech"),
				},
			});
			s.SwaggerDoc("v2", new OpenApiInfo
			{
				Title = "MelvinBankApi Doc",
				Version = "v2",
				Description = "MelvinBank Api Documentation",

			});
			var xmlFile = $"{typeof(Presentation.AssemblyReference).Assembly.GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
			s.IncludeXmlComments(xmlPath);
			s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Place to add JWT with Bearer",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});
			s.AddSecurityRequirement(new OpenApiSecurityRequirement()
		{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						},
						Name = "Bearer",
					},
					new List<string>()
				}
		});
		});
	}
}
