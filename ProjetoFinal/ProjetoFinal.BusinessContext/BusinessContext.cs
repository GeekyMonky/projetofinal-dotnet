using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal.BusinessContext
{
    public class BusinessContext: DbContext, IBusinessContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=SQL6033.site4now.net;Initial Catalog=db_abaa2b_eletrotudo;User Id=db_abaa2b_eletrotudo_admin;Password=4#qzUHW3yKMSX@t");
        }
    }
}
