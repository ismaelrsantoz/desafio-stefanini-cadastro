using CadastroPessoas.Api.Data;
using CadastroPessoas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using System.Linq; // Garante que o .Where() e outros métodos LINQ estejam disponíveis

namespace CadastroPessoas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public PessoasController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/Pessoas (com filtro)
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Pessoa>>> GetPessoas([FromQuery] string? nome, [FromQuery] string? cpf)
        {
            var query = _context.Pessoas.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                // Usando EF.Functions.Like para uma busca case-insensitive (melhor prática)
                query = query.Where(p => EF.Functions.Like(p.Nome, $"%{nome}%"));
            }

            if (!string.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.Cpf.Contains(cpf));
            }

            return await query.ToListAsync();
        }

        // GET: api/v1/Pessoas/5
        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Pessoa>> GetPessoa(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound();
            }
            return pessoa;
        }

        // POST: api/v1/Pessoas
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Pessoa>> PostPessoa(Pessoa pessoa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // --- MENSAGEM DE ERRO ATUALIZADA ---
            if (await _context.Pessoas.AnyAsync(p => p.Cpf == pessoa.Cpf))
            {
                return BadRequest("Este CPF já está cadastrado. Para editar, utilize a tela de Consulta.");
            }

            pessoa.DataCadastro = DateTime.UtcNow;
            pessoa.DataAtualizacao = DateTime.UtcNow;

            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPessoa), new { id = pessoa.Id, version = "1.0" }, pessoa);
        }
        
        // POST: api/v2/Pessoas
        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<Pessoa>> PostPessoaV2([FromBody] PessoaV2 pessoaV2)
        {
            if (pessoaV2.Endereco == null)
            {
                ModelState.AddModelError("Endereco", "O endereço é obrigatório para a v2.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // --- MENSAGEM DE ERRO ATUALIZADA ---
            if (await _context.Pessoas.AnyAsync(p => p.Cpf == pessoaV2.Cpf))
            {
                 return BadRequest("Este CPF já está cadastrado. Para editar, utilize a tela de Consulta.");
            }

            var pessoa = new Pessoa
            {
                Nome = pessoaV2.Nome,
                Cpf = pessoaV2.Cpf,
                DataNascimento = pessoaV2.DataNascimento,
                Email = pessoaV2.Email,
                Nacionalidade = pessoaV2.Nacionalidade,
                Naturalidade = pessoaV2.Naturalidade,
                Sexo = pessoaV2.Sexo,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow,
            };

            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();
            
            return Ok(pessoa);
        }

        // PUT: api/v1/Pessoas/5
        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> PutPessoa(int id, Pessoa pessoa)
        {
            if (id != pessoa.Id)
            {
                return BadRequest();
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // --- MENSAGEM DE ERRO ATUALIZADA ---
            if (await _context.Pessoas.AnyAsync(p => p.Cpf == pessoa.Cpf && p.Id != id))
            {
                return BadRequest("Este CPF já pertence a outro registro no sistema.");
            }
            
            var pessoaExistente = await _context.Pessoas.FindAsync(id);

            if (pessoaExistente == null)
            {
                return NotFound();
            }

            _context.Entry(pessoaExistente).CurrentValues.SetValues(pessoa);
            pessoaExistente.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        // DELETE: api/v1/Pessoas/5
        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeletePessoa(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound();
            }

            _context.Pessoas.Remove(pessoa);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

