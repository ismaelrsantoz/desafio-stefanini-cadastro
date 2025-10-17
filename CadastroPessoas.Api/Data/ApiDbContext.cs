using CadastroPessoas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CadastroPessoas.Api.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<Pessoa> Pessoas { get; set; }
        
        // Adicionei esta linha para que o Entity Framework saiba sobre a tabela de Utilizadores.
        public DbSet<User> Users { get; set; }
    }
}