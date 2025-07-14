using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace ProjetoFinal.Shared;
public class Product : BaseEntity
{
    [Key]
    [Column("ProductId")]
    public new int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<Image>? Images { get; set; }
    public ICollection<StockMovement>? StockMovements { get; set; }
}

