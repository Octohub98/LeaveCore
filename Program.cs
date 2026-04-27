using AutoMapper;
using LeaveCore.MiddleWares;
using LeaveCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Octohub.Chronicle.Extensions;
using Octohub.Chronicle.Utility;
using Octohub.Core.Middleware;
using System.Text;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();

    builder.Services.AddDbContext<LeaveContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("LeaveConnection")));

    builder.Services.AddHttpContextAccessor();

    var jwtKey = builder.Configuration["JwtSettings:Key"]
        ?? throw new InvalidOperationException("JwtSettings:Key is missing.");
    var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]
        ?? throw new InvalidOperationException("JwtSettings:Issuer is missing.");
    var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtIssuer,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddDIService();

    var mappingConfig = new MapperConfiguration(mc =>
    {
        mc.AddProfile(new MappingProfile());
    }, new LoggerFactory());
    IMapper mapper = mappingConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "LeaveCore : 1.0.0",
            Version = "v1",
            Description = "Leave management: types, entitlements, requests with approval workflow."
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"Enter your JWT token like this: Bearer xxxxx.yyyyy.zzzzz",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        c.AddSecurityRequirement(document => new()
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    });

    const string CorsPolicy = "AllowAngularDev";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: CorsPolicy, policy =>
        {
            policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin)) return false;
                try
                {
                    var uri = new Uri(origin);
                    if (uri.Host.EndsWith(".octohub.xyz")) return true;
                    if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) return true;
                }
                catch { return false; }
                return false;
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
    });

    builder.Configuration
        .AddJsonFile("appsettings.chronicle.json", optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    builder.Services.AddDetection();
    builder.Services.AddChronicleLogging(builder.Configuration);

    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseCors(CorsPolicy);
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseChronicleAuditLogging();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    await StartupErrorLogger.LogStartupErrorAsync(ex);
}
