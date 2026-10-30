using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;
using OrderProcessing.Shared.Storage;

namespace OrderProcessing.Phase1;

public class SyncOrderValidator
{
    private readonly SyncCustomerRepository _customerRepository;
    private readonly SyncProductRepository _productRepository;
    private readonly SyncCouponRepository _couponRepository;
    private readonly JsonFileStore _store;

    public SyncOrderValidator(
        SyncCustomerRepository customerRepository,
        SyncProductRepository productRepository,
        SyncCouponRepository couponRepository,
        JsonFileStore store)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _couponRepository = couponRepository;
        _store = store;
    }

    public void ValidateOrder(Order order)
    {
        Log.Info($"Validating Order #{order.Id}...");

        string? failureReason = null;

        Customer? customer =
            _customerRepository.GetById(order.CustomerId);

        if (customer == null)
        {
            failureReason = $"Customer #{order.CustomerId} not found";
        }
        else if (!customer.Isactive)
        {
            failureReason = "Customer is inactive";
        }

        if (failureReason == null)
        {
            foreach (OrderItem item in order.Items)
            {
                Product? product =
                    _productRepository.GetById(item.ProductId);

                if (product == null)
                {
                    failureReason = $"Product #{item.ProductId} not found";
                    break;
                }

                if (product.Stockquantity < item.Quantity)
                {
                    failureReason =
                        $"Insufficient stock for '{product.Name}'";
                    break;
                }
            }
        }

        if (failureReason == null && order.CouponCode != null)
        {
            Coupon? coupon =
                _couponRepository.GetByCode(order.CouponCode);

            if (coupon == null)
            {
                failureReason =
                    $"Coupon '{order.CouponCode}' does not exist";
            }
            else if (coupon.ExpiryDate.Date < DateTime.Today)
            {
                failureReason =
                    $"Coupon '{order.CouponCode}' has expired";
            }
        }

        if (failureReason == null)
        {
            order.Status = OrderStatus.Validated;
            order.FailureReason = null;

            Log.Success($"Order #{order.Id} validated.");
        }
        else
        {
            order.Status = OrderStatus.Rejected;
            order.FailureReason = failureReason;

            Log.Error($"Order #{order.Id} rejected: {failureReason}");
        }

        List<Order> orders = _store.Read<Order>(DataFiles.Orders);

        int index = orders.FindIndex(o => o.Id == order.Id);

        if (index >= 0)
            orders[index] = order;
        else
            orders.Add(order);

        _store.Write(DataFiles.Orders, orders);
    }
}