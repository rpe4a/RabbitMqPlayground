using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlayground.Services;

public class PublishSubscribeService
{
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private const bool _durable = true;
    private const string _publishSubscribeExchangeName = "PublishSubscribeExchange";
    private const string _publishSubscribeQueueOne = "PublishSubscribeQueueOne";
    private const string _publishSubscribeQueueTwo = "PublishSubscribeQueueTwo";

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
        model.ExchangeDeclare(_publishSubscribeExchangeName, ExchangeType.Fanout, true);
        model.QueueDeclare(_publishSubscribeQueueOne, _durable, false, false, null);
        model.QueueDeclare(_publishSubscribeQueueTwo, _durable, false, false, null);
        model.QueueBind(_publishSubscribeQueueOne, _publishSubscribeExchangeName, "");
        model.QueueBind(_publishSubscribeQueueTwo, _publishSubscribeExchangeName, "");
    }

    public void SendMessageToPublishSubscribeQueues(string message, IModel model)
    {
        var basicProperties = model.CreateBasicProperties();
        basicProperties.Persistent = true;
        byte[] payload = Encoding.UTF8.GetBytes(message);
        model.BasicPublish(_publishSubscribeExchangeName,"", basicProperties, payload);
    }

    public void ReceivePublishSubscribeMessageReceiverOne(IModel model)
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

        model.BasicConsume(_publishSubscribeQueueOne, false, consumer);
    }
    
    public void ReceivePublishSubscribeMessageReceiverTwo(IModel model)
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

        model.BasicConsume(_publishSubscribeQueueTwo, false, consumer);
    }
}