using CadastroPessoas.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Asp.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using Asp.Versioning.ApiExplorer; // Adicione este using

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("A chave JWT (Jwt:Key) não está configurada no appsettings.json");
}

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

// --- Seção de Configuração dos Serviços ---
builder.Services.AddControllers();
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

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

// Adiciona o configurador do Swagger que entende de versionamento
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    // A configuração do JWT para o botão "Authorize"
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

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
    });
});

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


// --- Seção de Construção e Pipeline da Aplicação ---
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // ATUALIZAÇÃO NO SWAGGER UI para criar o menu dropdown
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

// app.UseHttpsRedirection();
app.UseCors(myAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// --- CLASSE AUXILIAR PARA O SWAGGER ---
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