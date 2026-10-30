using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class SyncCouponRepository
{
    private readonly JsonFileStore _store;

    public SyncCouponRepository(JsonFileStore store)
    {
        _store = store;
    }

    public Coupon? GetByCode(string code)
    {
        Log.Info("SyncCouponRepository => Opening connection...");

        Thread.Sleep(200);

        Log.Info(
            $"SyncCouponRepository => Executing query for Coupon {code}...");

        int delay = Random.Shared.Next(300, 601);

        Thread.Sleep(delay);

        List<Coupon> coupons =
            _store.Read<Coupon>(DataFiles.Coupons);

        foreach (Coupon coupon in coupons)
        {
            if (string.Equals(
                coupon.Code,
                code,
                StringComparison.OrdinalIgnoreCase))
            {
                Log.Success(
                    $"SyncCouponRepository =>  Coupon {code} found.");

                return coupon;
            }
        }

        Log.Error($"SyncCouponRepository => Coupon {code} not found.");

        return null;
    }
}