// See https://aka.ms/new-console-template for more information

using Microsoft.AspNetCore.SignalR.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqps://yszyrutc:ZsFrcEvJ0MJBv3zj6kV9i5vGEECOm6LU@sparrow.rmq.cloudamqp.com/yszyrutc");
IConnection connection = factory.CreateConnection();

HubConnection HubConnection = new HubConnectionBuilder().WithUrl("https://localhost:7017/messageHub", options =>
    {
        options.WebSocketConfiguration = conf =>
        {
            conf.RemoteCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
        };
        options.HttpMessageHandlerFactory = factory => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
        };
    }).Build();


await HubConnection.StartAsync();
using (connection)
{



    using IModel chanel = connection.CreateModel();
    chanel.QueueDeclare("messageQueue", false, false, false);
    EventingBasicConsumer consumer = new(chanel);
    chanel.BasicConsume("messageQueue", true, consumer);

    consumer.Received += async (s, e) =>
    {
        //mail sending
        Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
        await HubConnection.InvokeAsync("SendMessageAsync", Encoding.UTF8.GetString(e.Body.Span));
    };
    Console.ReadKey();
}