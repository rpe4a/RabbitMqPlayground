using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlaygroundLibrary;

public class WorkerQueueService
{
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private string _exchangeName = "";
    private const string _queue = "WorkerQueue";
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

    public void SendMessage(string message, IModel model)
    {
        var basicProperties = model.CreateBasicProperties();
        basicProperties.Persistent = true;
        byte[] payload = Encoding.UTF8.GetBytes(message);
        model.BasicPublish(_exchangeName, _queue, basicProperties, payload);
    }

    public void ReceiveMessages(IModel model)
    {
        model.BasicQos(0, 1, false);
        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
        consumer.Received += (consumer, @event) =>
        {
            var body = @event.Body;
            var message = Encoding.UTF8.GetString(body.Span);
        
            Console.WriteLine(message);
            Thread.Sleep(500);
            model.BasicAck(@event.DeliveryTag, false);
        };

        model.BasicConsume(_queue, false, consumer);
    }
}