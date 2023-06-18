using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlaygroundLibrary;

public class HeaderService
{
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private const bool _durable = true;
    private const string headerExchange = "HeaderExchange";
    private const string headerQueueOne = "HeaderQueueOne";
    private const string headerQueueTwo = "HeaderQueueTwo";
    private const string headerQueueThree = "HeaderQueueThree";

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
        model.ExchangeDeclare(headerExchange, ExchangeType.Headers, true);
        model.QueueDeclare(headerQueueOne, _durable, false, false, null);
        model.QueueDeclare(headerQueueTwo, _durable, false, false, null);
        model.QueueDeclare(headerQueueThree, _durable, false, false, null);

        var bindingsOneHeaders = new Dictionary<string, object>
        {
            {"x-match", "all"},
            {"category", "animal"},
            {"type", "mammal"}
        };
        model.QueueBind(headerQueueOne, headerExchange, "", bindingsOneHeaders);

        var bindingsTwoHeaders = new Dictionary<string, object>
        {
            {"x-match", "any"},
            {"category", "animal"},
            {"type", "insect"}
        };
        model.QueueBind(headerQueueTwo, headerExchange, "", bindingsTwoHeaders);
        
        var bindingsThreeHeaders = new Dictionary<string, object>
        {
            {"x-match", "any"},
            {"category", "plant"},
            {"type", "flower"}
        };
        model.QueueBind(headerQueueThree, headerExchange, "", bindingsThreeHeaders);
    }

    public void SendHeaderMessage(string message, Dictionary<string,object> headers, IModel model)
    {
        var basicProperties = model.CreateBasicProperties();
        basicProperties.Persistent = _durable;
        basicProperties.Headers = headers;
        byte[] payload = Encoding.UTF8.GetBytes(message);
        model.BasicPublish(headerExchange, "", basicProperties, payload);
    }

    public void ReceiveHeaderMessageReceiverOne(IModel model)
    {
        model.BasicQos(0, 1, false);
        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
        consumer.Received += (_, @event) =>
        {
            var body = @event.Body;
            var message = Encoding.UTF8.GetString(body.Span);
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append("Message from queue: ").Append(message).Append(". ");
            foreach (string headerKey in @event.BasicProperties.Headers.Keys)
            {
                byte[] value = @event.BasicProperties.Headers[headerKey] as byte[];
                messageBuilder.Append("Header key: ").Append(headerKey).Append(", value: ").Append(Encoding.UTF8.GetString(value)).Append("; ");
            }
            Console.WriteLine(message);
            Thread.Sleep(Random.Shared.Next(1000));
            model.BasicAck(@event.DeliveryTag, false);
        };

        model.BasicConsume(headerQueueOne, false, consumer);
    }

    public void ReceiveHeaderMessageReceiverTwo(IModel model)
    {
        model.BasicQos(0, 1, false);
        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
        consumer.Received += (_, @event) =>
        {
            var body = @event.Body;
            var message = Encoding.UTF8.GetString(body.Span);
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append("Message from queue: ").Append(message).Append(". ");
            foreach (string headerKey in @event.BasicProperties.Headers.Keys)
            {
                byte[] value = @event.BasicProperties.Headers[headerKey] as byte[];
                messageBuilder.Append("Header key: ").Append(headerKey).Append(", value: ").Append(Encoding.UTF8.GetString(value)).Append("; ");
            }
            Console.WriteLine(message);
            Thread.Sleep(Random.Shared.Next(1000));
            model.BasicAck(@event.DeliveryTag, false);
        };

        model.BasicConsume(headerQueueTwo, false, consumer);
    }

    public void ReceiveHeaderMessageReceiverThree(IModel model)
    {
        model.BasicQos(0, 1, false);
        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
        consumer.Received += (_, @event) =>
        {
            var body = @event.Body;
            var message = Encoding.UTF8.GetString(body.Span);
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append("Message from queue: ").Append(message).Append(". ");
            foreach (string headerKey in @event.BasicProperties.Headers.Keys)
            {
                byte[] value = @event.BasicProperties.Headers[headerKey] as byte[];
                messageBuilder.Append("Header key: ").Append(headerKey).Append(", value: ").Append(Encoding.UTF8.GetString(value)).Append("; ");
            }
            Console.WriteLine(message);
            Thread.Sleep(Random.Shared.Next(1000));
            model.BasicAck(@event.DeliveryTag, false);
        };

        model.BasicConsume(headerQueueThree, false, consumer);
    }
}