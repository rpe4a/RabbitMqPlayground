using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlaygroundLibrary;

public class ScatterGatherService
{
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private const bool _durable = true;
    private const string scatterGatherExchange = "ScatterGatherExchange";
    private const string scatterGatherQueueOne = "ScatterGatherQueueOne";
    private const string scatterGatherQueueTwo = "ScatterGatherQueueTwo";
    private const string scatterGatherQueueThree = "ScatterGatherQueueThree";

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
        model.ExchangeDeclare(scatterGatherExchange, ExchangeType.Topic, true);
        model.QueueDeclare(scatterGatherQueueOne, _durable, false, false, null);
        model.QueueDeclare(scatterGatherQueueTwo, _durable, false, false, null);
        model.QueueDeclare(scatterGatherQueueThree, _durable, false, false, null);

        model.QueueBind(scatterGatherQueueOne, scatterGatherExchange, "cars");
        model.QueueBind(scatterGatherQueueOne, scatterGatherExchange, "trucks");

        model.QueueBind(scatterGatherQueueTwo, scatterGatherExchange, "cars");
        model.QueueBind(scatterGatherQueueTwo, scatterGatherExchange, "aeroplanes");
        model.QueueBind(scatterGatherQueueTwo, scatterGatherExchange, "buses");

        model.QueueBind(scatterGatherQueueThree, scatterGatherExchange, "cars");
        model.QueueBind(scatterGatherQueueThree, scatterGatherExchange, "buses");
        model.QueueBind(scatterGatherQueueThree, scatterGatherExchange, "tractors");
    }

    public Task<string> SendScatterGatherMessage(string message, string rotingKey, string minimalResponseCount,
        TimeSpan fromMinutes, IModel model)
    {
        CancellationTokenSource cts = new CancellationTokenSource(fromMinutes);
        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

        cts.Token.Register(() => tcs.TrySetCanceled());

        if (string.IsNullOrEmpty(_responseQueue))
        {
            _responseQueue = model.QueueDeclare().QueueName;
        }

        consumerTCS = tcs;
        if (consumer == null)
        {
            model.BasicQos(0, 1, false);
            consumer = new EventingBasicConsumer(model);
            consumer.Received += (_, @event) =>
            {
                var body = @event.Body;
                var response = Encoding.UTF8.GetString(body.Span);
                consumerTCS.SetResult($"Response: {response}");
            };


            model.BasicConsume(_responseQueue, true, consumer);
        }

        IBasicProperties props = model.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = _responseQueue;
        var messageBytes = Encoding.UTF8.GetBytes(message);

        model.BasicPublish(exchange: string.Empty, rotingKey, props, messageBytes);
        // while (!cts.IsCancellationRequested)
        // {
        //     BasicDeliverEventArgs deliveryArguments;
        //     consumer..Dequeue(500, out deliveryArguments);
        //     if (deliveryArguments != null && deliveryArguments.BasicProperties != null
        //                                   && deliveryArguments.BasicProperties.CorrelationId == correlationId)
        //     {
        //         string response = Encoding.UTF8.GetString(deliveryArguments.Body);
        //         responses.Add(response);
        //         if (responses.Count >= minResponses)
        //         {
        //             break;
        //         }
        //     }
        // }
        return tcs.Task;
    }

    private string? _responseQueue;
    private TaskCompletionSource<string>? consumerTCS;
    private EventingBasicConsumer? consumer;

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

        model.BasicConsume(scatterGatherQueueOne, false, consumer);
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

        model.BasicConsume(scatterGatherQueueTwo, false, consumer);
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

        model.BasicConsume(scatterGatherQueueThree, false, consumer);
    }
}