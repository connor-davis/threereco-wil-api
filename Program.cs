using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using three_api.Lib.Services;
using three_api.Lib.Helpers;
using System.Text;
using three_api.Lib.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddTransient<AuthenticationService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "3rEco API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    x.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
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
            new string[] {}
        }
    };

    x.AddSecurityRequirement(securityRequirement);
});

// JWT

builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        AuthenticationSettings.GenerateKeys();

        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticationSettings.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
    .AddPolicy("Businesses", policy => policy.RequireRole("Admin", "Business"))
    .AddPolicy("Collectors", policy => policy.RequireRole("Admin", "Business", "Collector"))
    .AddPolicy("Products", policy => policy.RequireRole("Admin", "Business"))
    .AddPolicy("Collections", policy => policy.RequireRole("Admin", "Business", "Collector"))
    .AddPolicy("Collections Export", policy => policy.RequireRole("Admin"))
    .AddPolicy("Guest", policy => policy.RequireRole("Guest"));

builder.Services
    .AddDbContext<DatabaseContext>(options =>
        options.UseNpgsql(
            builder
                .Configuration
                .GetConnectionString("PostgresDB")
        )
    );

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.json", "3rEco API V1");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app
    .MapControllers();

// Automatically apply migrations at app startup (use only in development!)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

    dbContext.Database.Migrate();  // Automatically applies pending migrations
}

app.Run();
