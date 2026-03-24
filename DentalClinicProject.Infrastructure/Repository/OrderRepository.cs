using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddPaymentAsync(int orderId, AddPaymentDTO paymentDto)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            if (!order.Items.Any())
                throw new Exception("Order has no items to pay for.");

            if (order.Payments.Any(p => p.Status == PaymentStatus.Paid))
                throw new Exception("Order already paid.");

            if (order.Status != OrderStatus.Processing)
                throw new Exception("Order must be in processing state.");

            if (Math.Abs(paymentDto.Amount - order.TotalAmount) > 0.01m)
                throw new Exception("Invalid payment amount.");

            if (order.UserId != paymentDto.CustomerId)
                throw new Exception("Unauthorized payment.");


            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var payment = new Payment
                {
                    Amount = paymentDto.Amount,
                    Currency = paymentDto.Currency,
                    Description = paymentDto.Description,
                    CustomerId = paymentDto.CustomerId,
                    OrderId = order.Id,
                    Status = PaymentStatus.Paid,
                    PaymentMethod = PaymentMethod.Cash,
                    PaidAt = DateTime.UtcNow
                };

                await _context.Payments.AddAsync(payment);

                order.Status = OrderStatus.Completed;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return null;

            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<List<OrderDTO>> GetOrdersForDeliveryAsync(int deliveryId, DateTime deliveryDate)
        {
            if (deliveryDate == default)
                throw new Exception("Invalid delivery date.");

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
                throw new Exception("No orders found for the specified delivery.");

            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task<List<OrderDTO>> GetOrdersByUserAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.DeliveryDate)
                .ToListAsync();

            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task UpdateOrderItemsAsync(int orderId, List<CreateOrderItemDTO> newItems)
        {
            if (newItems == null || !newItems.Any())
                throw new Exception("Order must contain items.");

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status != OrderStatus.Processing)
                throw new Exception("Can only update items in processing orders.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in order.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                    }
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
                        throw new Exception($"Product with Id {item.ProductId} not found.");

                    var product = products[item.ProductId];
                    if (product.Stock < item.Quantity)
                        throw new Exception($"Not enough stock for product {product.Name}");
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
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Completed)
                throw new Exception("Cannot update status of a completed or cancelled order.");

            if (status == OrderStatus.Cancelled && order.Payments.Any(p => p.Status == PaymentStatus.Paid))
                throw new Exception("Cannot cancel an order that has been paid.");

            order.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task<OrderDTO> CreateOrderAsync(CreateOrderDTO dto, string userId)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new Exception("Order must contain items.");

            // 1. Validate Delivery
            var deliveryExists = await _context.Deliveries
                .AnyAsync(d => d.Id == dto.DeliveryId);

            if (!deliveryExists)
                throw new Exception("Invalid DeliveryId.");

            // 2. Merge duplicate items
            var mergedItems = dto.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            var productIds = mergedItems.Select(i => i.ProductId).ToList();

            // 3. Get products
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // 4. Validate products + stock
            foreach (var item in mergedItems)
            {
                if (!products.ContainsKey(item.ProductId))
                    throw new Exception($"Product with Id {item.ProductId} not found.");

                var product = products[item.ProductId];

                if (product.Stock < item.Quantity)
                    throw new Exception($"Not enough stock for product {product.Name}");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 5. Create Order
                var order = new Order
                {
                    UserId = userId,
                    Status = OrderStatus.Processing,
                    DeliveryId = dto.DeliveryId,
                    DeliveryDate = dto.DeliveryDate,
                    Items = mergedItems.Select(i =>
                    {
                        var product = products[i.ProductId];

                        // 6. Reduce stock
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

                return _mapper.Map<OrderDTO>(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status == OrderStatus.Completed)
                throw new Exception("Cannot cancel a completed order.");

            if (order.Payments.Any(p => p.Status == PaymentStatus.Paid))
                throw new Exception("Cannot cancel an order that has been paid.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in order.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                    }
                }

                order.Status = OrderStatus.Cancelled;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}