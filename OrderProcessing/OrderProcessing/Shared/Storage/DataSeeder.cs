using OrderProcessing.Shared.Models;

namespace OrderProcessing.Shared.Storage;

public class DataSeeder
{
    private readonly string _dataDirectory;
    private readonly JsonFileStore _store;

    public DataSeeder(string dataDirectory)
    {
        _dataDirectory = dataDirectory;
        _store = new JsonFileStore(dataDirectory);
    }

    public async Task ResetAsync(CancellationToken ct)
    {
        if (Directory.Exists(_dataDirectory))
            Directory.Delete(_dataDirectory, true);

        Directory.CreateDirectory(_dataDirectory);

        Directory.CreateDirectory(
            Path.Combine(_dataDirectory, "invoices"));

        Directory.CreateDirectory(
            Path.Combine(_dataDirectory, "reports"));

        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "Ahmed Ali", Email = "ahmed.ali@example.com", Isactive = true, Loyaltytier = Loyaltytier.Gold },
            new() { Id = 2, Name = "Sara Mohamed", Email = "sara.mohamed@example.com", Isactive = true, Loyaltytier = Loyaltytier.Silver },
            new() { Id = 3, Name = "Omar Hassan", Email = "omar.hassan@example.com", Isactive = true, Loyaltytier = Loyaltytier.Bronze },
            new() { Id = 4, Name = "Mona Ibrahim", Email = "mona.ibrahim@example.com", Isactive = false, Loyaltytier = Loyaltytier.Silver },
            new() { Id = 5, Name = "Youssef Adel", Email = "youssef.adel@example.com", Isactive = true, Loyaltytier = Loyaltytier.Gold },
            new() { Id = 6, Name = "Nour Khaled", Email = "nour.khaled@example.com", Isactive   = true, Loyaltytier = Loyaltytier.Bronze },
            new() { Id = 7, Name = "Karim Samir", Email = "karim.samir@example.com", Isactive = true, Loyaltytier = Loyaltytier.Silver },
            new() { Id = 8, Name = "Laila Mahmoud", Email = "laila.mahmoud@example.com", Isactive = true, Loyaltytier = Loyaltytier.Gold },
            new() { Id = 9, Name = "Hassan Tarek", Email = "hassan.tarek@example.com", Isactive = true, Loyaltytier = Loyaltytier.Bronze },
            new() { Id = 10, Name = "Dina Fathy", Email = "dina.fathy@example.com", Isactive = true, Loyaltytier = Loyaltytier.Silver }
        };

        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 1200.00m, Stockquantity = 10, WeightKg = 2.2m },
            new() { Id = 2, Name = "Smartphone", Price = 800.00m, Stockquantity = 15, WeightKg = 0.4m },
            new() { Id = 3, Name = "Wireless Mouse", Price = 25.00m, Stockquantity = 2, WeightKg = 0.1m },
            new() { Id = 4, Name = "Mechanical Keyboard", Price = 90.00m, Stockquantity = 1, WeightKg = 0.9m },
            new() { Id = 5, Name = "USB-C Hub", Price = 45.00m, Stockquantity = 30, WeightKg = 0.2m },
            new() { Id = 6, Name = "Headphones", Price = 150.00m, Stockquantity = 12, WeightKg = 0.3m },
            new() { Id = 7, Name = "27\" Monitor", Price = 350.00m, Stockquantity  = 6, WeightKg = 5.0m },
            new() { Id = 8, Name = "Webcam", Price = 70.00m, Stockquantity = 20, WeightKg = 0.2m }
        };

        var coupons = new List<Coupon>
        {
            new() { Code = "SAVE10", DiscountPercent = 10, ExpiryDate = DateTime.Today.AddDays(180) },
            new() { Code = "WELCOME15", DiscountPercent = 15, ExpiryDate = DateTime.Today.AddDays(90) },
            new() { Code = "FLASH20", DiscountPercent = 20, ExpiryDate = DateTime.Today.AddDays(7) },
            new() { Code = "OLD20", DiscountPercent = 20, ExpiryDate = DateTime.Today.AddDays(-30) }
        };

        var incomingOrders = new List<Order>
        {
            CreateOrder(1, 1, (1, 1), (3, 1), "SAVE10"),
            CreateOrder(2, 2, (2, 1)),
            CreateOrder(3, 3, (5, 2), (8, 1), "WELCOME15"),
            CreateOrder(4, 4, (6, 1)),
            CreateOrder(5, 5, (7, 1)),
            CreateOrder(6, 6, (5, 1), coupon: "FLASH20"),
            CreateOrder(7, 7, (6, 1), (8, 1)),
            CreateOrder(8, 8, (1, 1), coupon: "SAVE10"),
            CreateOrder(9, 9, (3, 5)),
            CreateOrder(10, 10, (4, 1)),
            CreateOrder(11, 1, (4, 1)),
            CreateOrder(12, 2, (2, 1), coupon: "OLD20"),
            CreateOrder(13, 3, (6, 2)),
            CreateOrder(14, 5, (8, 2), (5, 1), "WELCOME15"),
            CreateOrder(15, 99, (2, 1)),
            CreateOrder(16, 6, (7, 1), (5, 1)),
            CreateOrder(17, 7, (2, 1), coupon: "SAVE10"),
            CreateOrder(18, 8, (6, 1), coupon: "FAKE50"),
            CreateOrder(19, 9, (8, 1)),
            CreateOrder(20, 10, (1, 1), (6, 1), "FLASH20")
        };

        await SaveSeedAsync(DataFiles.Customers, customers, ct);
        await SaveSeedAsync(DataFiles.Products, products, ct);
        await SaveSeedAsync(DataFiles.Coupons, coupons, ct);
        await SaveSeedAsync(DataFiles.IncomingOrders, incomingOrders, ct);
        await SaveSeedAsync(DataFiles.Orders, new List<Order>(), ct);
        await SaveSeedAsync(DataFiles.Payments, new List<PaymentTransaction>(), ct);
        await SaveSeedAsync(DataFiles.Emails, new List<EmailMessage>(), ct);
        await SaveSeedAsync(DataFiles.EmailRetry, new List<EmailMessage>(), ct);
    }

    private async Task SaveSeedAsync<T>(
        string fileName,
        List<T> data,
        CancellationToken ct)
    {
        await _store.WriteAsync(fileName, data, ct);

        Console.WriteLine(
            $"[Seeder] {fileName} -> {data.Count} records");
    }

    private static Order CreateOrder(
        int id,
        int customerId,
        (int productId, int quantity) item1,
        (int productId, int quantity)? item2 = null,
        string? coupon = null)
    {
        var items = new List<OrderItem>
        {
            new()
            {
                ProductId = item1.productId,
                Quantity = item1.quantity,
                UnitPrice = 0
            }
        };

        if (item2.HasValue)
        {
            items.Add(new OrderItem
            {
                ProductId = item2.Value.productId,
                Quantity = item2.Value.quantity,
                UnitPrice = 0
            });
        }

        return new Order
        {
            Id = id,
            CustomerId = customerId,
            Items = items,
            CouponCode = coupon,
            SubTotal = 0,
            Discount = 0,
            ShippingCost = 0,
            TotalAmount = 0,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now,
            FailureReason = null,
            TransactionId = null
        };
    }
}