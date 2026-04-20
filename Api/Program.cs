using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Vulpes.Perpendicularity.Api.Configuration;
using Vulpes.Perpendicularity.Api.Middleware;
using Vulpes.Perpendicularity.Api.Services;
using Vulpes.Perpendicularity.Core.RegistrationExtensions;
using Vulpes.Perpendicularity.Infrastructure.Mongo;
using Vulpes.Perpendicularity.Infrastructure.RegistrationExtensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add JWT Configuration
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfiguration>()
    ?? throw new InvalidOperationException("JWT configuration is missing");
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("JWT Token validated successfully");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine($"Authorization header received: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}...");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();  // Only if serving from same origin
    });
});

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new LowercaseParameterTransformer()));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var version = Vulpes.Perpendicularity.Core.Configuration.ApplicationConfiguration.Version;
    options.SwaggerDoc(version, new OpenApiInfo
    {
        Title = "Perpendicularity API",
        Version = version
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add middleware
builder.Services.AddTransient<UserContextMiddleware>();

MongoConfigurator.Configure();

// Dependencies.
_ = builder.Services
    .InjectCore()
    .InjectInfrastructure()
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
var version = Vulpes.Perpendicularity.Core.Configuration.ApplicationConfiguration.Version;
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"Perpendicularity API {version}"));

// Serve static files from UI dist folder
var uiDistPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Ui", "dist");
if (Directory.Exists(uiDistPath))
{
    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = new PhysicalFileProvider(uiDistPath)
    });

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uiDistPath)
    });
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Add custom user context middleware
app.UseMiddleware<UserContextMiddleware>();

app.MapControllers();

// Fallback to index.html for client-side routing (Vue Router)
if (Directory.Exists(uiDistPath))
{
    app.MapFallback(async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(uiDistPath, "index.html"));
    });
}

app.Run();