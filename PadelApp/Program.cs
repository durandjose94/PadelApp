using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PadelApp.Datos;
using PadelApp.PadelMapper;
using PadelApp.Repositorios;
using PadelApp.Repositorios.IRepositorios;
using PadelApp.Servicios;
using PadelApp.Servicios.IServicios;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Area de servicios - INICIO

builder.Services.AddControllers();

// 1. Obtener la cadena de conexion de appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Configurar el DbContext - MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString) // Esto detecta si usas MySQL 8.0, 5.7, etc.
    );
});

// Cambia .UseMySql por .UseSqlServer
/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));*/

// Agrega Swagger para documentacion de la API

builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description =
            "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
            "Ingresa la palabra 'Bearer' seguida de un espacio y luego tu token.\r\n\r\n" +
            "Ejemplo: \"Bearer trtrtgfgsdgsdfsdfsdfw\""
            ,
            Name = "Authorization",
            In = ParameterLocation.Header,
            Scheme = "Bearer"
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
                    },
                    Scheme = "ouath2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    }
);

//Soporta para CORS (Cross-Origin Resource Sharing) - Permitir solicitudes desde cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaCors", builder =>
    {
        builder.WithOrigins("http://localhost:8100", "https://my-padel-app.vercel.app", "capacitor://localhost")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

//Agregamos repositorios e interfaces a la inyeccion de dependencias

builder.Services.AddScoped<ISedeRepositorio, SedeRepositorio>();
builder.Services.AddScoped<IPistaRepositorio, PistaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IReservaRepositorio, ReservaRepositorio>();
builder.Services.AddScoped<IRecuperarContraseñaRepositorio, RecuperarContraseñaRepositorio>();
builder.Services.AddScoped<IAnuncioRepositorio, AnuncioRepositorio>();
builder.Services.AddScoped<IClubRepositorio, ClubRepositorio>();
builder.Services.AddScoped<IInvitacionRepositorio, InvitacionRepositorio>();

//Agregamos servicios e interfaces a la inyeccion de dependencias
builder.Services.AddScoped<IComprobanteServicio, ComprobanteServicio>();
builder.Services.AddScoped<IEmailServicio, EmailServicio>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//Agregar AutoMapper
builder.Services.AddAutoMapper(typeof(PadelMapper));

//Agregar autenticacion y autorizacion (si es necesario)
builder.Services.AddAuthentication
    (
        x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    ).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }
    );

// Area de servicios - FIN

var app = builder.Build();

// Area de middlewares - INICIO

app.UseSwagger(); // Habilita Swagger

app.UseSwaggerUI(); // Habilita la UI de Swagger

//Soporta para CORS (Cross-Origin Resource Sharing)
app.UseCors("PoliticaCors");

// 2. Autenticacion 
app.UseAuthentication();

// 3. Autorizacion 
app.UseAuthorization();

app.MapControllers();

// Area de middlewares - FIN

app.Run();
