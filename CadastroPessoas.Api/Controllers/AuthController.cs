using CadastroPessoas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CadastroPessoas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // --- VALIDAÇÃO DA CHAVE SECRETA ---
            var jwtKey = _configuration["Jwt:Key"];
            if (jwtKey == null)
            {
                // Logar o erro ou retornar um erro 500 para indicar má configuração do servidor
                return StatusCode(StatusCodes.Status500InternalServerError, "A chave JWT não está configurada no servidor.");
            }
            // --- FIM DA VALIDAÇÃO ---

            // Para este desafio, vamos usar um usuário "hardcoded" (fixo no código).
            if (model.Username == "admin" && model.Password == "password123")
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

                var token = new JwtSecurityToken(
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}