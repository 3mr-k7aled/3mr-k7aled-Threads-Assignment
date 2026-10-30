
using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class SyncCustomerRepository
{
    private readonly JsonFileStore _store;

    public SyncCustomerRepository(JsonFileStore store)
    {
        _store = store;
    }

    public Customer? GetById(int id)
    {
        Log.Info("SyncCustomerRepository => Opening connection...");

        Thread.Sleep(200);

        Log.Info(
            $"SyncCustomerRepository => Executing query for Customer #{id}...");

        int delay = Random.Shared.Next(300, 601);

        Thread.Sleep(delay);

        List<Customer> customers =
            _store.Read<Customer>(DataFiles.Customers);

        foreach (Customer customer in customers)
        {
            if (customer.Id == id)
            {
                Log.Success(
                    $"SyncCustomerRepository => Customer #{id} found ({customer.Name})");

                return customer;
            }
        }

        Log.Error(
            $"SyncCustomerRepository => Customer #{id} not found.");

        return null;
    }
}