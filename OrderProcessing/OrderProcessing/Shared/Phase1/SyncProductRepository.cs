
using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class SyncProductRepository
{
    private readonly JsonFileStore _store;

    public SyncProductRepository(JsonFileStore store)
    {
        _store = store;
    }

    public Product? GetById(int id)
    {
        Log.Info("SyncProductRepository => Opening connection...");

        Thread.Sleep(200);

        Log.Info(
            $"SyncProductRepository => Executing query for Product #{id}...");

        int delay = Random.Shared.Next(300, 601);

        Thread.Sleep(delay);

        List<Product> products =
            _store.Read<Product>(DataFiles.Products);

        foreach (Product product in products)
        {
            if (product.Id == id)
            {
                Log.Success(
                    $"SyncProductRepository => Product #{id} found ({product.Name})");

                return product;
            }
        }

        Log.Error($"SyncProductRepository => Product #{id} not found.");

        return null;
    }
}