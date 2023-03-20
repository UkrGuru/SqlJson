using System.ComponentModel.DataAnnotations;

namespace SqlJsonAppDemo.Data;

public class ProductDto
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(40)]
    public string? ProductName { get; set; }

    [Required]
    public string? CategoryName { get; set; }

    [Required]
    [StringLength(20)]
    public string? QuantityPerUnit { get; set; }

    public decimal? UnitPrice { get; set; }

    public short? UnitsInStock { get; set; }

    public short? UnitsOnOrder { get; set; }

    public short? ReorderLevel { get; set; }

    public bool? Discontinued { get; set; }
}
