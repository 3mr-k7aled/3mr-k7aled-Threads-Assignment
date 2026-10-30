using System.Diagnostics;
using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class Phase1Runner
{
    private readonly DataSeeder _seeder;
    private readonly IncomingOrderRepository _incomingOrderRepository;
    private readonly SyncOrderValidator _syncValidator;
    private readonly OrderValidator _asyncValidator;
    private readonly OrderRepository _orderRepository;

    public Phase1Runner(
        DataSeeder seeder,
        IncomingOrderRepository incomingOrderRepository,
        SyncOrderValidator syncValidator,
        OrderValidator asyncValidator,
        OrderRepository orderRepository)
    {
        _seeder = seeder;
        _incomingOrderRepository = incomingOrderRepository;
        _syncValidator = syncValidator;
        _asyncValidator = asyncValidator;
        _orderRepository = orderRepository;
    }

    public async Task RunAsync(CancellationToken ct)
    {
       
        await _seeder.ResetAsync(ct);

        List<Order> syncOrders =
            await _incomingOrderRepository.GetAllAsync(ct);

        Log.Info("Starting synchronous validation...");

        Stopwatch syncWatch = Stopwatch.StartNew();

        foreach (Order order in syncOrders)
        {
            _syncValidator.ValidateOrder(order);
        }

        syncWatch.Stop();

        Log.Success(
            $"Synchronous validation finished in {syncWatch.Elapsed.TotalSeconds:F2} seconds.");

        await _seeder.ResetAsync(ct);

      
        List<Order> asyncOrders =
            await _incomingOrderRepository.GetAllAsync(ct);

        Log.Info("Starting asynchronous validation...");

        Stopwatch asyncWatch = Stopwatch.StartNew();

        foreach (Order order in asyncOrders)
        {
            await _asyncValidator.ValidateOrderAsync(order, ct);
            await _orderRepository.SaveAsync(order, ct);
        }

        asyncWatch.Stop();

        Log.Success(
            $"Asynchronous validation finished in {asyncWatch.Elapsed.TotalSeconds:F2} seconds.");

        List<Order> validatedOrders =
            await _orderRepository.GetByStatusAsync(
                OrderStatus.Validated, ct);

        List<Order> rejectedOrders =
            await _orderRepository.GetByStatusAsync(
                OrderStatus.Rejected, ct);

        List<Order> allOrders =
            await _orderRepository.GetAllAsync(ct);

        Console.WriteLine();
        Console.WriteLine("===== Validation Summary =====");
        Console.WriteLine($"Total Orders: {allOrders.Count}");
        Console.WriteLine($"Validated: {validatedOrders.Count}");
        Console.WriteLine($"Rejected: {rejectedOrders.Count}");

        foreach (Order order in rejectedOrders)
        {
            Console.WriteLine(
                $"- Order #{order.Id}: {order.FailureReason}");
        }

        Console.WriteLine(
            $"Sync Time: {syncWatch.Elapsed.TotalSeconds:F2} seconds");

        Console.WriteLine(
            $"Async Time: {asyncWatch.Elapsed.TotalSeconds:F2} seconds");

        Console.WriteLine(
            $"orders.json now contains {allOrders.Count} records.");
    }
}