using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CadastroPessoas.Api.Validation
{
    public class CpfValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Se o valor não for uma string ou for nulo/vazio, a validação [Required] já cuida disso.
            if (value is not string cpf || string.IsNullOrEmpty(cpf))
            {
                return ValidationResult.Success;
            }

            // Remove caracteres não numéricos
            var cpfNumerico = Regex.Replace(cpf, @"[^\d]", "");

            if (cpfNumerico.Length != 11)
            {
                return new ValidationResult("O CPF deve conter exatamente 11 dígitos.");
            }
            
            return ValidationResult.Success;
        }
    }
}