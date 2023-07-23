using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlaygroundLibrary;

public class BadMessageService
{
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private string _exchangeName = "";
    private const string _queue = "BadMessageMessageQueue";
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

            Console.WriteLine("Message from queue: {0}", message);
            Random random = new Random();
            int i = random.Next(0, 2);

            //pretend that message cannot be processed and must be rejected
            if (i == 1) //reject the message and discard completely
            {
                Console.WriteLine("Rejecting and discarding message {0}", message);
                model.BasicReject(@event.DeliveryTag, false);
            }
            else //reject the message but push back to queue for later re-try
            {
                if (@event.BasicProperties.Headers != null
                    && @event.BasicProperties.Headers.ContainsKey("retry")
                    && (int)@event.BasicProperties.Headers["retry"] > 2)
                {
                    Console.WriteLine("MAX RETRY COUNT. Rejecting and discarding message {0}", message);
                    model.BasicReject(@event.DeliveryTag, false);
                    return;
                }

                var basicProperties = model.CreateBasicProperties();
                basicProperties.Persistent = true;
                if (@event.BasicProperties.Headers != null && @event.BasicProperties.Headers.ContainsKey("retry"))
                {
                    @event.BasicProperties.Headers["retry"] = (int) @event.BasicProperties.Headers["retry"] + 1;
                    basicProperties.Headers = @event.BasicProperties.Headers;
                }
                else
                {
                    basicProperties.Headers = new Dictionary<string, object>() {{"retry", 0}};
                }
                
                Console.WriteLine("Rejecting message and putting it back to the queue: {0}, retry: {1}", message, basicProperties.Headers["retry"]);

                model.BasicPublish(@event.Exchange, @event.RoutingKey, basicProperties, @event.Body);
                model.BasicAck(@event.DeliveryTag, false);
            }
        };
        model.BasicConsume(_queue, false, consumer);
    }
}