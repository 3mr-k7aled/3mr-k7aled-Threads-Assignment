using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class ProductRepository
{
    private readonly JsonFileStore _store;

    public ProductRepository(JsonFileStore store)
    {
        _store = store;
    }

    public async Task<Product?> GetByIdAsync( int id , CancellationToken ct)
    {
        Log.Info("ProductRepository => Opening connection...");

        await Task.Delay(200, ct);

        Log.Info(
            $"ProductRepository => Executing query for Product #{id}...");

        int delay = Random.Shared.Next(300, 601);

        await Task.Delay(delay, ct);

        List<Product> products =
            await _store.ReadAsync<Product>(DataFiles.Products, ct);

        foreach (Product product in products)
        {
            if (product.Id == id)
            {
                Log.Success(
                    $"ProductRepository => Product #{id} found ({product.Name})");

                return product;
            }
        }

        Log.Error($"ProductRepository =>     Product #{id} not found.");

        return null;
    }
    public async Task<bool> ReserveStockAsync(
    int productId,
    int qty,
    CancellationToken ct)
    {
        Log.Info("ProductRepository => Opening connection...");
        await Task.Delay(200, ct);

        Log.Info("ProductRepository =>  Beginning transaction...");
        await Task.Delay(100, ct);

        Log.Info($"ProductRepository => Updating stock for Product #{productId}...");

        bool reserved = false;

        await _store.UpdateAsync<Product>(
            DataFiles.Products,
            products =>
            {
                foreach (Product product in products)
                {
                    if (product.Id == productId &&
                        product.Stockquantity >= qty)
                    {
                        product.Stockquantity -= qty;
                        reserved = true;
                        break;
                    }
                }
            },
            ct);

        await Task.Delay(400, ct);

        Log.Info("ProductRepository => Committing transaction...");
        await Task.Delay(200, ct);

        if (reserved)
            Log.Success($"ProductRepository => Stock reserved for Product #{productId}.");
        else
            Log.Error($"ProductRepository => Could not reserve stock for Product #{productId}.");

        return reserved;
    }
}