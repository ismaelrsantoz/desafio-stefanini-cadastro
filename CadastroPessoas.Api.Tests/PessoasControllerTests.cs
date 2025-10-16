using Xunit;
using Microsoft.EntityFrameworkCore;
using CadastroPessoas.Api.Data;
using CadastroPessoas.Api.Controllers;
using CadastroPessoas.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CadastroPessoas.Api.Tests
{
    public class PessoasControllerTests
    {
        private ApiDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new ApiDbContext(options);
        }

        [Fact]
        public async Task GetPessoas_DeveRetornarListaDePessoas()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.Pessoas.Add(new Pessoa { Id = 1, Nome = "Teste 1", Cpf = "11111111111", DataNascimento = new System.DateTime(2000, 1, 1) });
            dbContext.Pessoas.Add(new Pessoa { Id = 2, Nome = "Teste 2", Cpf = "22222222222", DataNascimento = new System.DateTime(2001, 1, 1) });
            await dbContext.SaveChangesAsync();
            var controller = new PessoasController(dbContext);

            // Act
            var result = await controller.GetPessoas();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Pessoa>>>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Pessoa>>(actionResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task PostPessoa_DeveCriarNovaPessoa()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new PessoasController(dbContext);
            var novaPessoa = new Pessoa { Nome = "Novo Usuario", Cpf = "33333333333", DataNascimento = new System.DateTime(1990, 5, 15) };

            // Act
            var result = await controller.PostPessoa(novaPessoa);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(1, await dbContext.Pessoas.CountAsync());
        }

        [Fact]
        public async Task PostPessoa_DeveRetornarBadRequest_QuandoCpfJaExiste()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var pessoaExistente = new Pessoa { Nome = "Usuario Antigo", Cpf = "44444444444", DataNascimento = new System.DateTime(1980, 1, 1) };
            dbContext.Pessoas.Add(pessoaExistente);
            await dbContext.SaveChangesAsync();
            var controller = new PessoasController(dbContext);
            var pessoaDuplicada = new Pessoa { Nome = "Usuario Novo", Cpf = "44444444444", DataNascimento = new System.DateTime(1995, 1, 1) };

            // Act
            var result = await controller.PostPessoa(pessoaDuplicada);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("CPF já cadastrado no sistema.", actionResult.Value);
        }

        [Fact]
        public async Task GetPessoa_DeveRetornarPessoa_QuandoIdExiste()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var pessoaTeste = new Pessoa { Id = 1, Nome = "Pessoa Encontrada", Cpf = "55555555555", DataNascimento = new System.DateTime(2000, 1, 1) };
            dbContext.Pessoas.Add(pessoaTeste);
            await dbContext.SaveChangesAsync();
            var controller = new PessoasController(dbContext);

            // Act
            var result = await controller.GetPessoa(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Pessoa>>(result);
            var model = Assert.IsType<Pessoa>(actionResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task GetPessoa_DeveRetornarNotFound_QuandoIdNaoExiste()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new PessoasController(dbContext);

            // Act
            var result = await controller.GetPessoa(99);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeletePessoa_DeveRemoverPessoa_QuandoIdExiste()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var pessoaParaDeletar = new Pessoa { Id = 1, Nome = "Pessoa a Deletar", Cpf = "66666666666", DataNascimento = new System.DateTime(2000, 1, 1) };
            dbContext.Pessoas.Add(pessoaParaDeletar);
            await dbContext.SaveChangesAsync();
            var controller = new PessoasController(dbContext);

            // Act
            var result = await controller.DeletePessoa(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await dbContext.Pessoas.FindAsync(1));
        }

        [Fact]
        public async Task DeletePessoa_DeveRetornarNotFound_QuandoIdNaoExiste()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new PessoasController(dbContext);

            // Act
            var result = await controller.DeletePessoa(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public async Task PutPessoa_DeveAtualizarPessoa_QuandoDadosSaoValidos()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var pessoaOriginal = new Pessoa { Id = 1, Nome = "Nome Antigo", Cpf = "77777777777", DataNascimento = new System.DateTime(2000, 1, 1) };
            dbContext.Pessoas.Add(pessoaOriginal);
            await dbContext.SaveChangesAsync();

            var controller = new PessoasController(dbContext);
            var pessoaAtualizada = new Pessoa { Id = 1, Nome = "Nome Novo", Cpf = "77777777777", DataNascimento = new System.DateTime(2000, 1, 1) };

            // Act
            var result = await controller.PutPessoa(1, pessoaAtualizada);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var pessoaDoBanco = await dbContext.Pessoas.FindAsync(1);
            Assert.NotNull(pessoaDoBanco); // <-- Garante que não é nulo
            Assert.Equal("Nome Novo", pessoaDoBanco.Nome); // <-- O aviso vai desaparecer
        }

        [Fact]
        public async Task PutPessoa_DeveRetornarNotFound_QuandoIdNaoExiste()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new PessoasController(dbContext);
            var pessoaInexistente = new Pessoa { Id = 99, Nome = "Ninguem", Cpf = "99999999999", DataNascimento = new System.DateTime(2000, 1, 1) };

            // Act
            var result = await controller.PutPessoa(99, pessoaInexistente);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutPessoa_DeveRetornarBadRequest_QuandoIdDoUrlEhDiferenteDoIdDoCorpo()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new PessoasController(dbContext);
            var pessoa = new Pessoa { Id = 2, Nome = "Teste", Cpf = "88888888888", DataNascimento = new System.DateTime(2000, 1, 1) };

            // Act
            // Note que o ID na URL (1) é diferente do ID no corpo do objeto (2)
            var result = await controller.PutPessoa(1, pessoa);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutPessoa_DeveRetornarBadRequest_QuandoCpfJaExisteEmOutraPessoa()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            // Pessoa A com CPF 111
            dbContext.Pessoas.Add(new Pessoa { Id = 1, Nome = "Pessoa A", Cpf = "11111111111", DataNascimento = new System.DateTime(2000, 1, 1) });
            // Pessoa B com CPF 222
            var pessoaB = new Pessoa { Id = 2, Nome = "Pessoa B", Cpf = "22222222222", DataNascimento = new System.DateTime(2001, 1, 1) };
            dbContext.Pessoas.Add(pessoaB);
            await dbContext.SaveChangesAsync();

            var controller = new PessoasController(dbContext);
            
            // Tenta atualizar a Pessoa B para ter o mesmo CPF da Pessoa A
            pessoaB.Cpf = "11111111111";

            // Act
            var result = await controller.PutPessoa(2, pessoaB);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("CPF já cadastrado no sistema para outra pessoa.", badRequestResult.Value);
        }
    }
}