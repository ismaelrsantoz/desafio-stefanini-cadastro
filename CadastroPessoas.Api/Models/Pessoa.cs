using System.ComponentModel.DataAnnotations;
using CadastroPessoas.Api.Validation; // Garante que os atributos customizados sejam reconhecidos

namespace CadastroPessoas.Api.Models
{
    public class Pessoa
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string? Sexo { get; set; }

        [EmailAddress(ErrorMessage = "O endereço de e-mail informado não é válido.")]
        public string? Email { get; set; }

        [Required]
        [PastDate] 
        public DateTime DataNascimento { get; set; }

        public string? Naturalidade { get; set; }

        public string? Nacionalidade { get; set; }

        [Required]
        [CpfValidation]
        public string Cpf { get; set; } = string.Empty;

        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}

