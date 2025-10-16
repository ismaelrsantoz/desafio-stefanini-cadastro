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
    }
}