using Api.ClinicaMedica.AccesoDatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// SERVICES: 

// Evitar referencias cíclicas y serializaciones innecesarias
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Obtener la cadena de conexión
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Registrar ApplicationDbContext en el contenedor de servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuración de AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .WithMethods("GET", "POST", "PUT", "DELETE")  // Métodos específicos
               .AllowAnyHeader()
               .WithExposedHeaders("Content-Disposition"); // Para descargar archivos
    });

    options.AddPolicy("AllowVercel", builder =>
    {
        builder.WithOrigins("https://turno-facil.vercel.app")
               .WithMethods("GET", "POST", "PUT", "DELETE")  // Métodos específicos
               .AllowAnyHeader()
               .WithExposedHeaders("Content-Disposition");
    });
});

// Configurar autenticación con JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:ClaveSecreta"]))
        };
    });

// Habilitar autorización
builder.Services.AddAuthorization();

var app = builder.Build();

// MIDDLEWARES:

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configuración CORS (antes de `UseRouting`)
app.UseCors(app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing") ? "AllowAll" : "AllowVercel");

app.UseHttpsRedirection();
app.UseRouting();

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
