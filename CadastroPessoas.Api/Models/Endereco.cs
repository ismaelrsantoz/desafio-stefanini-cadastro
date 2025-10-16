using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Api.Models
{
    public class Endereco
    {
        [Required]
        public string Logradouro { get; set; } = string.Empty;

        [Required]
        public string Numero { get; set; } = string.Empty;

        public string? Complemento { get; set; }

        [Required]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        public string Cidade { get; set; } = string.Empty;

        [Required]
        public string Estado { get; set; } = string.Empty;

        [Required]
        public string Cep { get; set; } = string.Empty;
    }
}