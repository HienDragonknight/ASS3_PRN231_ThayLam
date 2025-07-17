using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.DTOs;
using System.Security.Claims;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderRepository _orderRepository;
        private readonly OrchidRepository _orchidRepository;

        public OrderController(OrderRepository orderRepository, OrchidRepository orchidRepository)
        {
            _orderRepository = orderRepository;
            _orchidRepository = orchidRepository;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            var orderDtos = orders.Select(MapOrderToDto).ToList();
            return Ok(orderDtos);
        }

        // GET: api/Order/my-orders
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders(int id)
        {
         

            var orders = await _orderRepository.GetUserOrdersAsync(id);
            var orderDtos = orders.Select(MapOrderToDto).ToList();
            return Ok(orderDtos);
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

         

            return Ok(MapOrderToDto(order));
        }

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            // Convert DTO to entity
            var orderDetails = new List<BusinessObjects.Entities.OrderDetail>();
            foreach (var item in orderDto.OrderDetails)
            {
                // Get orchid to verify it exists and get price
                var orchid = await _orchidRepository.GetOrchidAsync(item.OrchidId);
                if (orchid == null) return BadRequest($"Orchid with ID {item.OrchidId} not found");

                orderDetails.Add(new BusinessObjects.Entities.OrderDetail
                {
                    OrchidId = item.OrchidId,
                    Quantity = item.Quantity,
                    Price = orchid.Price
                });
            }

            // Create order
            var order = await _orderRepository.CreateOrderAsync(2, orderDetails);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, MapOrderToDto(order));
        }

        // PUT: api/Order/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status)) return BadRequest("Status cannot be empty");

            var success = await _orderRepository.UpdateOrderStatusAsync(id, status);
            if (!success) return NotFound();

            return NoContent();
        }

        // Helper method to map Order entity to DTO
        private OrderResponseDto MapOrderToDto(BusinessObjects.Entities.Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                AccountId = order.AccountId ?? 0,
                UserEmail = order.Account?.Email,
                OrderDate = order.OrderDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                OrderStatus = order.OrderStatus ?? "Unknown",
                TotalAmount = order.TotalAmount ?? 0,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailResponseDto
                {
                    Id = od.Id,
                    OrchidId = od.OrchidId ?? 0,
                    OrchidName = od.Orchid?.OrchidName,
                    Price = od.Price ?? 0,
                    Quantity = od.Quantity ?? 0,
                    Subtotal = (od.Price ?? 0) * (od.Quantity ?? 0)
                }).ToList()
            };
        }
    }
}