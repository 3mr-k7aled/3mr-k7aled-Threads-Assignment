
using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class OrderRepository
{
    private readonly JsonFileStore _store;

    public OrderRepository(JsonFileStore store)
    {
        _store = store;
    }

    public async Task SaveAsync(Order order, CancellationToken ct)
    {
        Log.Info("OrderRepository => Opening connection...");
        await Task.Delay(200, ct);

        Log.Info($"OrderRepository => Beginning transaction for Order #{order.Id}...");
        await Task.Delay(100, ct);

        Log.Info($"OrderRepository => Saving Order #{order.Id}...");
        await Task.Delay(400, ct);

        await _store.UpdateAsync<Order>(
            DataFiles.Orders,
            orders =>
            {
                int index = orders.FindIndex(o => o.Id == order.Id);

                if (index >= 0)
                    orders[index] = order;
                else
                    orders.Add(order);
            },
            ct);

        Log.Info("OrderRepository => Committing transaction...");
        await Task.Delay(200, ct);

        Log.Success($"OrderRepository => Order #{order.Id} saved.");
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken ct)
    {
        Log.Info("OrderRepository => Opening connection...");
        await Task.Delay(200, ct);

        Log.Info("OrderRepository => Executing query for all orders...");
        int delay = Random.Shared.Next(300, 601);
        await Task.Delay(delay, ct);

        List<Order> orders =
            await _store.ReadAsync<Order>(DataFiles.Orders, ct);

        Log.Success($"OrderRepository => Retrieved {orders.Count} orders.");

        return orders;
    }

    public async Task<List<Order>> GetByStatusAsync(
        OrderStatus status,
        CancellationToken ct)
    {
        Log.Info("OrderRepository => Opening connection...");
        await Task.Delay(200, ct);

        Log.Info($"OrderRepository => Executing query for status {status}...");
        int delay = Random.Shared.Next(300, 601);
        await Task.Delay(delay, ct);

        List<Order> orders =
            await _store.ReadAsync<Order>(DataFiles.Orders, ct);

        List<Order> result = new();

        foreach (Order order in orders)
        {
            if (order.Status == status)
                result.Add(order);
        }

        Log.Success(
            $"OrderRepository => Retrieved {result.Count} orders with status {status}.");

        return result;
    }
}