using System;
using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Api.Validation
{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || !(value is DateTime))
            {
                return ValidationResult.Success; // A validação [Required] cuida de valores nulos.
            }

            var dateValue = (DateTime)value;

            // 1. Validação de data futura
            if (dateValue > DateTime.Now)
            {
                return new ValidationResult("A data de nascimento não pode ser uma data futura.");
            }

            // 2. Validação de idade máxima (ex: 120 anos)
            var minDate = DateTime.Now.AddYears(-120);
            if (dateValue < minDate)
            {
                return new ValidationResult("A data de nascimento resulta em uma idade superior a 120 anos, o que é inválido.");
            }

            return ValidationResult.Success;
        }
    }
}

