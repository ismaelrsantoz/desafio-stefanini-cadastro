using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Api.Models
{
    public class PessoaV2
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;
        
        public string? Sexo { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public DateTime DataNascimento { get; set; }

        public string? Naturalidade { get; set; }

        public string? Nacionalidade { get; set; }

        [Required]
        public string Cpf { get; set; } = string.Empty;

        [Required]
        public Endereco Endereco { get; set; } = new();

        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}