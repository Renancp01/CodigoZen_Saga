using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Orders.Contracts.Order;
using Orders.Models;

namespace Orders.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrder([FromBody] OrderRequest orderRequest)
    {
        var orderId = Guid.NewGuid();

        await _publishEndpoint.Publish(new OrderSubmitted
        {
            OrderId = orderId,
            CustomerNumber = orderRequest.CustomerNumber
            // Outros detalhes do pedido
        });

        return Accepted(new { OrderId = orderId });
    }
}
