using Hajz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hajz.Data
{
    public class HajzDbContext : IdentityDbContext<User>
    {
        public HajzDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           




        }


    
        public DbSet<CachRE> CachRE { get; set; }
        public DbSet<Newhajz> Newhajz { get; set; }
        public DbSet<Typerev> Typerev { get; set; }
        public DbSet<Typemor> Typemor { get; set; }

        public DbSet<Itemrev> Itemrev { get; set; }

        public DbSet<Expenses> Expenses { get; set; }


        


    }
}
