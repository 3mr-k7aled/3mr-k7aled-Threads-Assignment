
using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class CouponRepository
{
    private readonly JsonFileStore _store;

    public CouponRepository(JsonFileStore store)
    {
        _store = store;
    }

    public async Task<Coupon?> GetByCodeAsync(
        string code,
        CancellationToken ct)
    {
        Log.Info("CouponRepository => Opening connection...");

        await Task.Delay(200, ct);

        Log.Info(
            $"CouponRepository => Executing query for Coupon {code}...");

        int delay = Random.Shared.Next(300, 601);

        await Task.Delay(delay, ct);

        List<Coupon> coupons =
            await _store.ReadAsync<Coupon>(DataFiles.Coupons, ct);

        foreach (Coupon coupon in coupons)
        {
            if (string.Equals(
                coupon.Code,
                code,
                StringComparison.OrdinalIgnoreCase))
            {
                Log.Success(
                    $"CouponRepository => Coupon {code} found.");

                return coupon;
            }
        }

        Log.Error($"CouponRepository => Coupon {code} not found.");

        return null;
    }
}