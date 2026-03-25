using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentalClinicProject.API.Controllers.Core
{
    //[Authorize]
    public class OrderController : BaseController
    {
        public OrderController(IUnitOfWork work) : base(work) { }

        private string GetCurrentUid() =>
            User.FindFirst("uid")?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        private IActionResult OkResponse(object? data, string? message = null) =>
            Ok(new { success = true, message, data });

        private IActionResult ErrorResponse(string message, int statusCode = 400) =>
            StatusCode(statusCode, new { success = false, message });

        [HttpGet("get-with-delivery-id")]
        //[Authorize(Roles = "Admin,DelivaryMan")]
        public async Task<IActionResult> GetOrdersForDelivery(int deliveryId, DateTime deliveryDate)
        {
            try
            {
                var orders = await work.OrderRepository.GetOrdersForDeliveryAsync(deliveryId, deliveryDate);
                if (!orders.Any())
                    return ErrorResponse("No orders found for the specified delivery.", 404);

                return OkResponse(orders);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpGet("get-by-id")]
        //[Authorize(Roles = "Admin,User,Patient,Doctor,DelivaryMan")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await work.OrderRepository.GetOrderByIdAsync(orderId);
                if (order == null)
                    return ErrorResponse("Order not found.", 404);

                return OkResponse(order);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpGet("get-by-user")]
        [Authorize(Roles = "User,Patient")]
        public async Task<IActionResult> GetOrdersByUser()
        {
            try
            {
                var userId = GetCurrentUid();
                var orders = await work.OrderRepository.GetOrdersByUserAsync(userId);
                if (!orders.Any())
                    return ErrorResponse("No orders found for the current user.", 404);

                return OkResponse(orders);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPost("create")]
        //[Authorize(Roles = "User,Patient")]
        public async Task<IActionResult> CreateOrder(List<CreateOrderItemDTO> Items)
        {
            try
            {
                //var userId = GetCurrentUid();
                var order = await work.OrderRepository.CreateOrderAsync(Items, "user-1");
                return OkResponse(order, "Order created successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPut("cancel")]
        [Authorize(Roles = "User,Patient,Admin")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                await work.OrderRepository.CancelOrderAsync(orderId);
                return OkResponse(null, "Order cancelled successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPut("update-order-items")]
        [Authorize(Roles = "User,Patient")]
        public async Task<IActionResult> UpdateOrderItems(int orderId, List<CreateOrderItemDTO> items)
        {
            try
            {
                await work.OrderRepository.UpdateOrderItemsAsync(orderId, items);
                return OkResponse(null, "Order items updated successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPost("checkout-cart")]
        [Authorize(Roles = "User,Patient")]
        public async Task<IActionResult> CheckoutCart()
        {
            try
            {
                var userId = GetCurrentUid();
                var cart = await work.CartRepository.GetCartByUserIdAsync(userId, Role.User);
                if (cart == null || !cart.Items.Any())
                    return ErrorResponse("Cart is empty.", 404);

                List<CreateOrderItemDTO> createOrderItemDtos = cart.Items.Select(i => new CreateOrderItemDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList();

                var order = await work.OrderRepository.CreateOrderAsync(createOrderItemDtos, userId);

                await work.CartRepository.ClearCartAsync(cart.Id, userId, Role.User);

                return OkResponse(order, "Order created successfully from cart.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPut("mark-shipped")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkOrderAsShipped(int orderId)
        {
            try
            {
                await work.OrderRepository.MarkOrderAsShippedAsync(orderId);
                return OkResponse(null, "Order marked as shipped successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPut("mark-out-for-delivery")]
        //[Authorize(Roles = "Admin,DelivaryMan")]
        public async Task<IActionResult> MarkOrderAsOutForDelivery(int orderId)
        {
            try
            {
                await work.OrderRepository.MarkOrderAsOutForDeliveryAsync(orderId);
                return OkResponse(null, "Order marked as out for delivery successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPut("complete")]
        //[Authorize(Roles = "Admin,DelivaryMan")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            try
            {
                await work.OrderRepository.CompleteOrderAsync(orderId);
                return OkResponse(null, "Order completed successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPost("add-payment")]
        //[Authorize(Roles = "User,Patient")]
        public async Task<IActionResult> AddPayment(int orderId, AddPaymentDTO paymentDto)
        {
            try
            {
                await work.OrderRepository.AddPaymentAsync(orderId, paymentDto);
                return OkResponse(null, "Payment added successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPost("cash-payment-delivery")]
        [Authorize(Roles = "DelivaryMan,Admin")]
        public async Task<IActionResult> CashPaymentForDelivery(int orderId)
        {
            try
            {
                await work.OrderRepository.AddPaymentAsync(orderId, null, true);
                return OkResponse(null, "Cash payment completed successfully with delivery.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPut("confirm-payment")]
        //[Authorize(Roles = "Admin,DelivaryMan")]
        public async Task<IActionResult> ConfirmPayment(int paymentId)
        {
            try
            {
                await work.OrderRepository.ConfirmPaymentAsync(paymentId);
                return OkResponse(null, "Payment confirmed successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }
    }
}