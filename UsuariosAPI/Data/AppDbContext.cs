using UsuariosAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace UsuariosAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}