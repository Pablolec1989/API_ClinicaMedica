using Api.ClinicaMedica.AccesoDatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//SERVICES: 

// Evitar referencias cíclicas y serializaciones innecesarias
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    options.AddPolicy("AllowVercel", builder =>
    {
        builder.WithOrigins("https://turno-facil.vercel.app")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

//// Configurar autenticación con JWT
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:ClaveSecreta"]))
//        };
//    });

// Habilitar autorización
builder.Services.AddAuthorization();

var app = builder.Build();

// MIDDLEWARES:

// Configurar el pipeline de middleware
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// Configuración CORS
//app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "AllowVercel");

//app.UseCors("AllowAll");  // Asegúrate de que esta línea se ejecuta
//app.UseCors("AllowVercel");

//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
//    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
//    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

//    if (context.Request.Method == "OPTIONS")
//    {
//        context.Response.StatusCode = 200;
//        return;
//    }

//    await next();
//});

// Middleware de autenticación y autorización
app.UseAuthentication();  // Se agrega el middleware de autenticación
app.UseAuthorization();

app.MapControllers();

app.Run();
