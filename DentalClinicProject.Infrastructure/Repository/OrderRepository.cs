using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.Logging;
using DentalClinicProject.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAppLogger<OrderRepository> _logger;
        private static readonly Random _rand = new Random();

        public OrderRepository(ApplicationDbContext context, IMapper mapper, IAppLogger<OrderRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
        {
            _logger.LogOperationStarted(nameof(GetOrderByIdAsync), new { orderId });

            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogNotFound("Order", orderId);
                return null;
            }

            _logger.LogOperationCompleted(nameof(GetOrderByIdAsync), new { orderId });
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<List<OrderDTO>> GetOrdersForDeliveryAsync(int deliveryId, DateTime deliveryDate)
        {
            _logger.LogOperationStarted(nameof(GetOrdersForDeliveryAsync), new { deliveryId, deliveryDate });

            if (deliveryDate == default)
            {
                _logger.LogValidationError(nameof(GetOrdersForDeliveryAsync), "DeliveryDate is invalid (default value).");
                throw new Exception("Invalid delivery date.");
            }

            var start = deliveryDate.Date;
            var end = start.AddDays(1);

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .Include(o => o.Delivery)
                .Where(o => o.DeliveryId == deliveryId
                         && o.DeliveryDate >= start
                         && o.DeliveryDate < end)
                .ToListAsync();

            if (!orders.Any())
            {
                _logger.LogEmptyResult(nameof(GetOrdersForDeliveryAsync), new { deliveryId, deliveryDate });
                throw new Exception("No orders found for the specified delivery.");
            }

            _logger.LogOperationCompleted(nameof(GetOrdersForDeliveryAsync), new { deliveryId, Count = orders.Count });
            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task<List<OrderDTO>> GetOrdersByUserAsync(string userId)
        {
            _logger.LogOperationStarted(nameof(GetOrdersByUserAsync), new { userId });

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.DeliveryDate)
                .ToListAsync();

            if (!orders.Any())
                _logger.LogEmptyResult(nameof(GetOrdersByUserAsync), new { userId });
            else
                _logger.LogOperationCompleted(nameof(GetOrdersByUserAsync), new { userId, Count = orders.Count });

            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> CreateOrderAsync(List<CreateOrderItemDTO> Items, string userId)
        {
            if (Items == null || !Items.Any())
                throw new Exception("Order must contain items.");

            var mergedItems = Items
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            var productIds = mergedItems.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var item in mergedItems)
            {
                if (!products.ContainsKey(item.ProductId))
                    throw new Exception($"Product with Id {item.ProductId} not found.");

                var product = products[item.ProductId];
                if (product.Stock < item.Quantity)
                    throw new Exception($"Not enough stock for product {product.Name}");
            }
            var delivery = await _context.Deliveries
                .Where(d => d.IsApproved)
                .FirstOrDefaultAsync();

            if (delivery == null)
                throw new Exception("No available delivery slot found.");

            _logger.LogOperationStarted(nameof(CreateOrderAsync),
                new { userId, DeliveryId = delivery.Id, ItemCount = Items.Count });

            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            DateTime startDate = DateTime.UtcNow.AddDays(2);
            DateTime orderDate;
            bool exists;

            do
            {
                int randomDays = _rand.Next(0, 3);
                int randomHours = _rand.Next(0, 8);
                orderDate = startDate.AddDays(randomDays).AddHours(randomHours);

                exists = await _context.Orders
                    .AnyAsync(o => o.DeliveryId == delivery.Id && o.DeliveryDate == orderDate);

            } while (exists);

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Processing,
                DeliveryId = delivery.Id,
                DeliveryDate = orderDate,
                Items = mergedItems.Select(i =>
                {
                    var product = products[i.ProductId];
                    product.Stock -= i.Quantity;

                    return new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = i.Quantity,
                        Price = product.Price
                    };
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogOperationCompleted(nameof(CreateOrderAsync), new { OrderId = order.Id, userId });
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task CancelOrderAsync(int orderId)
        {
            _logger.LogOperationStarted(nameof(CancelOrderAsync), new { orderId });

            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogNotFound("Order", orderId);
                throw new Exception("Order not found.");
            }

            if (order.Status != OrderStatus.Processing)
            {
                _logger.LogBusinessRuleViolation(nameof(CancelOrderAsync),
                    $"Order {orderId} is already {order.Status.ToString()} and cannot be cancelled.");
                throw new Exception("Cannot cancel a completed order.");
            }

            if (order.Payments.Any(p => p.Status == PaymentStatus.Paid))
            {
                _logger.LogBusinessRuleViolation(nameof(CancelOrderAsync), $"Order {orderId} has a paid payment and cannot be cancelled.");
                throw new Exception("Cannot cancel an order that has been paid.");
            }

            var productIds = order.Items.Select(i => i.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            if(!products.Any())
            {
                _logger.LogNotFound("Products for Order", orderId);
                throw new Exception("Associated products not found for the order.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in order.Items)
                {
                    if (products.TryGetValue(item.ProductId, out var product))
                    {
                        product.Stock += item.Quantity;
                    }
                    else
                    {
                        _logger.LogNotFound("Product", item.ProductId);
                        throw new Exception($"Product with Id {item.ProductId} not found while cancelling order {orderId}.");
                    }
                }

                order.Status = OrderStatus.Cancelled;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogOperationCompleted(nameof(CancelOrderAsync), new { orderId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogException(nameof(CancelOrderAsync), ex, new { orderId });
                throw;
            }
        }

        public async Task UpdateOrderItemsAsync(int orderId, List<CreateOrderItemDTO> newItems)
        {
            _logger.LogOperationStarted(nameof(UpdateOrderItemsAsync), new { orderId, ItemCount = newItems?.Count });

            if (newItems == null || !newItems.Any())
            {
                _logger.LogValidationError(nameof(UpdateOrderItemsAsync), "newItems list is null or empty.");
                throw new Exception("Order must contain items.");
            }

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogNotFound("Order", orderId);
                throw new Exception("Order not found.");
            }

            if (order.Status != OrderStatus.Processing)
            {
                _logger.LogBusinessRuleViolation(nameof(UpdateOrderItemsAsync),
                    $"Order {orderId} is not in Processing state. Current: {order.Status}");
                throw new Exception("Can only update items in processing orders.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in order.Items)
                {
                    var product = await _context.Products
                                    .FirstOrDefaultAsync(x => x.Id == item.ProductId);
                    if (product != null)
                        product.Stock += item.Quantity;
                }

                var mergedItems = newItems
                    .GroupBy(i => i.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Quantity = g.Sum(x => x.Quantity)
                    }).ToList();

                var productIds = mergedItems.Select(i => i.ProductId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                foreach (var item in mergedItems)
                {
                    if (!products.ContainsKey(item.ProductId))
                    {
                        _logger.LogNotFound("Product", item.ProductId);
                        throw new Exception($"Product with Id {item.ProductId} not found.");
                    }

                    var product = products[item.ProductId];
                    if (product.Stock < item.Quantity)
                    {
                        _logger.LogBusinessRuleViolation(nameof(UpdateOrderItemsAsync), $"Insufficient stock for product '{product.Name}'. Requested: {item.Quantity}, Available: {product.Stock}");
                        throw new Exception($"Not enough stock for product {product.Name}");
                    }
                }

                order.Items.Clear();
                foreach (var item in mergedItems)
                {
                    var product = products[item.ProductId];
                    product.Stock -= item.Quantity;

                    order.Items.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        Price = product.Price
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogOperationCompleted(nameof(UpdateOrderItemsAsync), new { orderId, NewItemCount = mergedItems.Count });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogException(nameof(UpdateOrderItemsAsync), ex, new { orderId });
                throw;
            }
        }

        public async Task MarkOrderAsShippedAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            if (order.Status != OrderStatus.Processing)
                throw new InvalidOperationException("Order is not in Processing state.");

            bool hasOnlinePaid = order.Payments.Any(p => p.Status == PaymentStatus.Paid && p.PaymentMethod != PaymentMethod.Cash);
            bool hasCashOnDelivery = order.Payments.Any(p => p.PaymentMethod == PaymentMethod.Cash && p.Status == PaymentStatus.Pending);

            if (!hasOnlinePaid && !hasCashOnDelivery)
                throw new InvalidOperationException("Order is not paid or awaiting cash on delivery.");

            order.Status = OrderStatus.Shipped;
            await _context.SaveChangesAsync();
        }

        public async Task MarkOrderAsOutForDeliveryAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            if (order.Status != OrderStatus.Shipped)
                throw new InvalidOperationException("Order is not ready for delivery.");

            bool canDeliver = order.Payments.Any(p =>
                (p.PaymentMethod != PaymentMethod.Cash && p.Status == PaymentStatus.Paid) ||
                (p.PaymentMethod == PaymentMethod.Cash && p.Status == PaymentStatus.Pending)
            );

            if (!canDeliver)
                throw new InvalidOperationException("Order cannot be delivered. Payment not valid.");

            order.Status = OrderStatus.OutForDelivery;
            await _context.SaveChangesAsync();
        }

        public async Task CompleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            if (order.Status != OrderStatus.OutForDelivery)
                throw new InvalidOperationException("Order is not out for delivery.");

            var cashPayments = order.Payments
                .Where(p => p.PaymentMethod == PaymentMethod.Cash && p.Status == PaymentStatus.Pending);

            foreach (var payment in cashPayments)
            {
                payment.Status = PaymentStatus.Paid;
                payment.PaidAt = DateTime.UtcNow;
            }

            order.Status = OrderStatus.Completed;
            await _context.SaveChangesAsync();
        }

        public async Task AddPaymentAsync(int orderId, AddPaymentDTO? paymentDto = null, bool isCashOnDelivery = false)
        {
            _logger.LogOperationStarted(nameof(AddPaymentAsync), new { orderId, CustomerId = paymentDto?.CustomerId });

            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            if (!order.Items.Any())
                throw new Exception("Order has no items to pay for.");

            if(order.Payments.Any())
                throw new Exception("Order already found payment.");

            if (order.Payments.Any(p => p.Status == PaymentStatus.Paid))
                throw new Exception("Order already fully paid.");

            if (isCashOnDelivery)
            {
                var payment = new Payment
                {
                    Amount = order.TotalAmount,
                    Currency = "EGP",
                    CustomerId = order.UserId,
                    OrderId = order.Id,
                    Status = PaymentStatus.Pending,
                    PaymentMethod = PaymentMethod.Cash,
                    Description = $"Cash payment for order {order.Id} at delivery"
                };

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                _logger.LogOperationCompleted(nameof(AddPaymentAsync), new { orderId, Status = PaymentStatus.Pending });
            }
            else
            {
                if (order.Status != OrderStatus.Processing)
                    throw new Exception("Order must be in processing state to add payment.");

                if (paymentDto == null)
                    throw new Exception("Payment information is required.");

                if (order.UserId != paymentDto.CustomerId)
                    throw new Exception("Unauthorized payment.");

                if (!Enum.IsDefined(typeof(PaymentMethod), paymentDto.PaymentMethod))
                    throw new Exception("Invalid payment method.");

                if (Math.Abs(paymentDto.Amount - order.TotalAmount) > 0.01m)
                    throw new Exception("Payment amount does not match order total.");

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var status = PaymentStatus.Pending;

                    var payment = new Payment
                    {
                        Amount = paymentDto.Amount,
                        Currency = paymentDto.Currency,
                        CustomerId = paymentDto.CustomerId,
                        OrderId = order.Id,
                        Status = status,
                        PaymentMethod = paymentDto.PaymentMethod,
                        Description = $"Payment for order {order.Id} by customer {paymentDto.CustomerId}"
                    };

                    await _context.Payments.AddAsync(payment);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogOperationCompleted(nameof(AddPaymentAsync), new { orderId, Status = status });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogException(nameof(AddPaymentAsync), ex, new { orderId, paymentDto.CustomerId });
                    throw;
                }
            }
        }

        public async Task ConfirmPaymentAsync(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .ThenInclude(o => o.Payments)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Paid)
                throw new InvalidOperationException("Already paid.");

            payment.Status = PaymentStatus.Paid;
            payment.PaidAt = DateTime.UtcNow;

            var order = payment.Order;
            if (order.Payments.All(p => p.Status == PaymentStatus.Paid))
            {
                if (order.Status == OrderStatus.Processing)
                    order.Status = OrderStatus.Shipped;
                else if (order.Status == OrderStatus.OutForDelivery)
                    order.Status = OrderStatus.Completed;
            }

            await _context.SaveChangesAsync();
        }
    }
}