using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class CustomerRepository
{
    private readonly JsonFileStore _store;

    public CustomerRepository(JsonFileStore store)
    {
        _store = store;
    }

    public async Task<Customer?> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        Log.Info("CustomerRepository => Opening connection...");

        await Task.Delay(200, ct);

        Log.Info($"CustomerRepository => Executing query for Customer #{id}...");

        int delay = Random.Shared.Next(300, 601);

        await Task.Delay(delay, ct);

        List<Customer> customers =
            await _store.ReadAsync<Customer>(DataFiles.Customers, ct);

        foreach (Customer customer in customers)
        {
            if (customer.Id == id)
            {
                Log.Success(
                    $"CustomerRepository => Customer #{id} found ({customer.Name})");

                return customer;
            }
        }

        Log.Error($"CustomerRepository => Customer #{id} not found.");

        return null;
    }
}