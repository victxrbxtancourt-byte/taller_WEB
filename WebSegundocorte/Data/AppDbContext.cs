using Microsoft.EntityFrameworkCore;
using WebSegundocorte.Models;

namespace WebSegundocorte.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Producto> Productos => Set<Producto>();
    }
}