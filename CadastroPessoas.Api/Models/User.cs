namespace CadastroPessoas.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        // Inicializei as propriedades para garantir que nunca sejam nulas.
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}