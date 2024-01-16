using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromQuery]string Message)
        {
            ConnectionFactory factory = new();
            factory.Uri = new Uri("amqps://yszyrutc:ZsFrcEvJ0MJBv3zj6kV9i5vGEECOm6LU@sparrow.rmq.cloudamqp.com/yszyrutc");
            IConnection connection = factory.CreateConnection();
            using (connection)
            {
                using IModel chanel = connection.CreateModel();
                chanel.QueueDeclare("messageQueue",false,false,false);
                chanel.BasicPublish("","messageQueue",body:Encoding.UTF8.GetBytes(Message));
            }
            return Ok();
        }
    }
}
