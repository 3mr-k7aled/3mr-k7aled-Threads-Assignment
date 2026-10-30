
using OrderProcessing.Phase1;
using OrderProcessing.Shared.Storage;

string dataDirectory = Path.Combine(
    AppContext.BaseDirectory,
    "Data");

JsonFileStore store = new(dataDirectory);

DataSeeder seeder = new(dataDirectory);

CustomerRepository customerRepository = new(store);
ProductRepository productRepository = new(store);
CouponRepository couponRepository = new(store);
OrderRepository orderRepository = new(store);
IncomingOrderRepository incomingOrderRepository = new(store);

SyncCustomerRepository syncCustomerRepository = new(store);
SyncProductRepository syncProductRepository = new(store);
SyncCouponRepository syncCouponRepository = new(store);

OrderValidator asyncValidator = new(
    customerRepository,
    productRepository,
    couponRepository,
    orderRepository);

SyncOrderValidator syncValidator = new(
    syncCustomerRepository,
    syncProductRepository,
    syncCouponRepository,
    store);

Phase1Runner runner = new(
    seeder,
    incomingOrderRepository,
    syncValidator,
    asyncValidator,
    orderRepository);

await runner.RunAsync(CancellationToken.None);