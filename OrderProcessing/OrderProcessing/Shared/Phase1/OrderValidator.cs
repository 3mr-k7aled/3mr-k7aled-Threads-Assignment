
using OrderProcessing.Shared;
using OrderProcessing.Shared.Models;

namespace OrderProcessing.Phase1;

public class OrderValidator
{
    private readonly CustomerRepository _customerRepository;
    private readonly ProductRepository _productRepository;
    private readonly CouponRepository _couponRepository;
    private readonly OrderRepository _orderRepository;


public OrderValidator(
    CustomerRepository customerRepository,
    ProductRepository productRepository,
    CouponRepository couponRepository,
    OrderRepository orderRepository)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _couponRepository = couponRepository;
        _orderRepository = orderRepository;
    }

    public async Task ValidateOrderAsync(
        Order order,
        CancellationToken ct)
    {
        Log.Info($"Validating Order #{order.Id}...");

        string? failureReason = null;

        Customer? customer =
            await _customerRepository.GetByIdAsync(order.CustomerId, ct);

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
                    await _productRepository.GetByIdAsync(item.ProductId, ct);

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
                await _couponRepository.GetByCodeAsync(order.CouponCode, ct);

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

        await _orderRepository.SaveAsync(order, ct);
    }


}
