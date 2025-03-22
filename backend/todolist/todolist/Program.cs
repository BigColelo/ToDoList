using Microsoft.EntityFrameworkCore;
using todolist.Context;
using todolist.BL;
using todolist.DAO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Configura Swagger per utilizzare la documentazione XML
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    // Aggiungi configurazione OAuth2 per Swagger
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Password = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri($"{builder.Configuration["Keycloak:Authority"]}/protocol/openid-connect/token"),
                AuthorizationUrl = new Uri($"{builder.Configuration["Keycloak:Authority"]}/protocol/openid-connect/auth")
            }
        }
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new[] { "openid", "profile", "email" }
        }
    });
});

// Add this to your service configuration
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services for dependency injection
builder.Services.AddScoped<IActivitiesDAO, ActivitiesDAO>();
builder.Services.AddScoped<IActivitiesBL, ActivitiesBL>();

// Aggiungi queste righe dopo la registrazione dei servizi per Activities
builder.Services.AddScoped<IUserDAO, UserDAO>();
builder.Services.AddScoped<IUserBL, UserBL>();

// Add CORS policy if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.Audience = builder.Configuration["Keycloak:Audience"];
    options.MetadataAddress = builder.Configuration["Keycloak:MetadataAddress"];
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

// Add this before app.MapControllers()
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();