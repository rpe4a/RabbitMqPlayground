using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlaygroundLibrary;

public class RoutingKeyService
{
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private const bool _durable = true;
    private const string _routingKeyExchange = "RoutingKeyExchange";
    private const string _routingKeyQueueOne = "RoutingKeyQueueOne";
    private const string _routingKeyQueueTwo = "RoutingKeyQueueTwo";

    private const string _carsRoutingKey = "cars";
    private const string _trucksRoutingKey = "trucks";


    public IConnection GetRabbitMqConnection()
    {
        ConnectionFactory connectionFactory = new ConnectionFactory
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password
        };

        return connectionFactory.CreateConnection();
    }

    public void SetUpQueue(IModel model)
    {
        model.ExchangeDeclare(_routingKeyExchange, ExchangeType.Direct, true);
        model.QueueDeclare(_routingKeyQueueOne, _durable, false, false, null);
        model.QueueDeclare(_routingKeyQueueTwo, _durable, false, false, null);
        model.QueueBind(_routingKeyQueueOne, _routingKeyExchange, _carsRoutingKey);
        model.QueueBind(_routingKeyQueueTwo, _routingKeyExchange, _trucksRoutingKey);
    }

    public void SendRoutingMessage(string message, string routingKey, IModel model)
    {
        var basicProperties = model.CreateBasicProperties();
        basicProperties.Persistent = _durable;
        byte[] payload = Encoding.UTF8.GetBytes(message);
        model.BasicPublish(_routingKeyExchange, routingKey, basicProperties, payload);
    }

    public void ReceiveRoutingKeyMessageReceiverOne(IModel model)
    {
        model.BasicQos(0, 1, false);
        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
        consumer.Received += (_, @event) =>
        {
            var body = @event.Body;
            var message = Encoding.UTF8.GetString(body.Span);

            Console.WriteLine(message);
            Thread.Sleep(Random.Shared.Next(1000));
            model.BasicAck(@event.DeliveryTag, false);
        };

        model.BasicConsume(_routingKeyQueueOne, false, consumer);
    }

    public void ReceiveRoutingKeyMessageReceiverTwo(IModel model)
    {
        model.BasicQos(0, 1, false);
        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
        consumer.Received += (_, @event) =>
        {
            var body = @event.Body;
            var message = Encoding.UTF8.GetString(body.Span);

            Console.WriteLine(message);
            Thread.Sleep(Random.Shared.Next(1000));
            model.BasicAck(@event.DeliveryTag, false);
        };

        model.BasicConsume(_routingKeyQueueTwo, false, consumer);
    }
}