using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal.BusinessContext.Entities
{
    public class StockMovement: BaseEntity
    {
        [Key]
        [Column("StockMovementId")]
        public new int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
