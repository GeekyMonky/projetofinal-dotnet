using Microsoft.EntityFrameworkCore;
using ProjetoFinal.BusinessContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal.BusinessContext
{
    public class BusinessContext: DbContext, IBusinessContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=SQL6033.site4now.net;Initial Catalog=db_abaa2b_projetofinal;User Id=db_abaa2b_projetofinal_admin;Password=123456a!");
        }
      
    }
}
