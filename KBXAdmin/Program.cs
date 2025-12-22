using KBXAdmin.Application.Interfaces;
using KBXAdmin.Application.Services;
using KBXAdmin.Common.Security;
using KBXAdmin.Infrastructure.Persistence;
using KBXAdmin.Infrastructure.Repositories.Implementations;
using KBXAdmin.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ---------------------- DB CONTEXT ----------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

// ---------------------- REPOSITORIES ----------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILoginAuditLogRepository, LoginAuditLogRepository>();

// ---------------------- SERVICES ----------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// ---------------------- HELPERS ----------------------
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddHttpContextAccessor();

// ---------------------- CONTROLLERS ----------------------
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
        // or frontend port
      .AllowAnyHeader()
      .AllowAnyMethod();

    });
});

// ---------------------- JWT AUTH ----------------------
var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);

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
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// ---------------------- SWAGGER ----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "KBXAdmin API", Version = "v1" });

    // Add JWT support into Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ---------------------- APP BUILD ----------------------
var app = builder.Build();
// ---------------------- SWAGGER ----------------------
app.UseSwagger();
app.UseSwaggerUI();
// Routing
app.UseRouting();
//----------------------- CORS ---------------------------
app.UseCors("CorsPolicy");


// ---------------------- MIDDLEWARE ----------------------
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
