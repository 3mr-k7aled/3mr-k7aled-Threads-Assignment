
namespace OrderProcessing.Shared.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stockquantity { get; set; }
    public decimal WeightKg { get; set; }

    public Product()
    {
    }
}