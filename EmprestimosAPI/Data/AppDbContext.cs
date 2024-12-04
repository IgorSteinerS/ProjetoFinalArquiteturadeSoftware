using EmprestimosAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EmprestimosAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Emprestimo> Emprestimos { get; set; }
    }
}
