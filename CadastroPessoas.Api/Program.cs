using CadastroPessoas.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Asp.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using Asp.Versioning.ApiExplorer;
using CadastroPessoas.Api.Models;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("A chave JWT (Jwt:Key) não está configurada no appsettings.json");
}

// --- Configuração dos Serviços ---
builder.Services.AddControllers();
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// CORREÇÃO FINAL: Simplifiquei a política de CORS para permitir qualquer origem.
// Isto resolve os problemas de comunicação entre Vercel e Render.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// Configurei o versionamento da API para atender aos requisitos extras.
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Adicionei o Swagger e a configuração para suportar o versionamento e o JWT.
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});


// Implementei a autenticação JWT para proteger os endpoints.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


// --- Construção e Pipeline da Aplicação ---
var app = builder.Build();

// --- INÍCIO DA SEÇÃO DE SEEDING E CRIAÇÃO DA BASE DE DADOS ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApiDbContext>();

    // Uso o sistema de migrations para garantir que o banco de dados está atualizado.
    context.Database.Migrate();

    // Com a tabela 'Users' garantidamente criada, verifico se o utilizador admin existe.
    if (!context.Users.Any())
    {
        context.Users.Add(new User
        {
            Username = "admin",
            Password = "password123" 
        });
        context.SaveChanges();
    }
}
// --- FIM DA SEÇÃO ---


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.Services.GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions;
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

// A ordem dos middlewares é importante: CORS primeiro.
app.UseCors(); // ATIVA a política de CORS que definimos acima.

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Classe auxiliar para configurar o Swagger de forma que ele entenda as versões da API.
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo()
            {
                Title = $"CadastroPessoas.Api {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
            });
        }
    }
}