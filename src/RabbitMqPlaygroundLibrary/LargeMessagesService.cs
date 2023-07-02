using System.Net.Mime;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlaygroundLibrary;

public class LargeMessagesService
{
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private const string _queue = "LargeMessageBufferedQueue";
    private const bool _durable = true;

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
        model.QueueDeclare(_queue, _durable, false, false, null);
    }

    public void SendLargeMessage(byte[] payload, IModel model)
    {
        var basicProperties = model.CreateBasicProperties();
        basicProperties.Persistent = true;
        basicProperties.ContentType = MediaTypeNames.Text.Plain;
        model.BasicPublish("", _queue, basicProperties, payload);
    }

    public void ReceiveMessages(IModel model)
    {
        model.BasicQos(0, 1, false);
        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
        consumer.Received += (consumer, @event) =>
        {
            var body = @event.Body;
            string randomFileName = string.Concat(@"C:\Users\zapma\Downloads\", Guid.NewGuid(), ".csv");
            Console.WriteLine("Received message, will save it to {0}", randomFileName);
            File.WriteAllBytes(randomFileName, body.ToArray());


            model.BasicAck(@event.DeliveryTag, false);
        };

        model.BasicConsume(_queue, false, consumer);
    }
}