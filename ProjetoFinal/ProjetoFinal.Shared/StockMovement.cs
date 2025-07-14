using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal.Shared
{
    public class StockMovement: BaseEntity 
    {
        public new int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
