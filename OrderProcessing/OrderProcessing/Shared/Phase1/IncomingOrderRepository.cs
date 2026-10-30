using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class IncomingOrderRepository
{
    private readonly JsonFileStore _store;

    public IncomingOrderRepository(JsonFileStore store)
    {
        _store = store;
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken ct)
    {
        Log.Info("IncomingOrderRepository => Opening connection...");

        await Task.Delay(200, ct);

        Log.Info("IncomingOrderRepository => Executing query for incoming orders...");

        int delay = Random.Shared.Next(300, 601);

        await Task.Delay(delay, ct);

        List<Order> orders =
            await _store.ReadAsync<Order>(DataFiles.IncomingOrders, ct);

        Log.Success(
            $"IncomingOrderRepository => Retrieved {orders.Count} incoming orders.");

        return orders;
    }
}