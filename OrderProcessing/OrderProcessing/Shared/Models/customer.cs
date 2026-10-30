
namespace OrderProcessing.Shared.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Isactive { get; set; }
    public Loyaltytier Loyaltytier { get; set; }

    public Customer()
    {
    }
}