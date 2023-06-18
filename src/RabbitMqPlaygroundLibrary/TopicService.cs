using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlaygroundLibrary;

public class TopicService
{
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private const bool _durable = true;
    private const string topicExchange = "TopicExchange";
    private const string topicQueueOne = "TopicQueueOne";
    private const string topicQueueTwo = "TopicQueueTwo";
    private const string topicQueueThree = "TopicQueueThree";
    
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
        model.ExchangeDeclare(topicExchange, ExchangeType.Topic, true);
        model.QueueDeclare(topicQueueOne, _durable, false, false, null);
        model.QueueDeclare(topicQueueTwo, _durable, false, false, null);
        model.QueueDeclare(topicQueueThree, _durable, false, false, null);
        model.QueueBind(topicQueueOne, topicExchange, "*.world.*");
        model.QueueBind(topicQueueTwo, topicExchange, "#.world.#");
        model.QueueBind(topicQueueThree, topicExchange, "#.world");
    }

    public void SendTopicMessage(string message, string routingKey, IModel model)
    {
        var basicProperties = model.CreateBasicProperties();
        basicProperties.Persistent = _durable;
        byte[] payload = Encoding.UTF8.GetBytes(message);
        model.BasicPublish(topicExchange, routingKey, basicProperties, payload);
    }

    public void ReceiveTopicsMessageReceiverOne(IModel model)
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

        model.BasicConsume(topicQueueOne, false, consumer);
    }

    public void ReceiveTopicsMessageReceiverTwo(IModel model)
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

        model.BasicConsume(topicQueueTwo, false, consumer);
    }
    
    public void ReceiveTopicsMessageReceiverThree(IModel model)
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

        model.BasicConsume(topicQueueThree, false, consumer);
    }
}