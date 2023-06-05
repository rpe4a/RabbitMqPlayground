using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlaygroundLibrary;

public class RPCService
{
    private string? _responseQueue;
    private EventingBasicConsumer consumer;
    private TaskCompletionSource<string> consumerTCS;
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private const string _rpcQueueName = "RpcQueue";
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
        model.QueueDeclare(_rpcQueueName, _durable, false, false, null);
    }

    public void ReceiveMessages(IModel model)
    {
        model.BasicQos(0, 1, false);
        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
        consumer.Received += (_, @event) =>
        {
            string response = string.Empty;
            var body = @event.Body.ToArray();
            var reqProps = @event.BasicProperties;
            var replyProps = model.CreateBasicProperties();
            replyProps.CorrelationId = reqProps.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Request: {message}");
                var handleTime = Random.Shared.Next(2000);
                Thread.Sleep(handleTime);
                response = $"Request: {message}, Handle time: {handleTime}";
            }
            catch (Exception e)
            {
                Console.WriteLine($" [.] {e.Message}");
                response = $"Error: {e.Message}";
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                model.BasicPublish(exchange: string.Empty,
                    routingKey: reqProps.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBytes);
                model.BasicAck(@event.DeliveryTag, false);
            }
        };

        model.BasicConsume(_rpcQueueName, false, consumer);
    }

    public Task<string> SendRpcMessageToQueue(string message, IModel model, TimeSpan fromMinutes)
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

        model.BasicPublish(exchange: string.Empty, routingKey: _rpcQueueName, basicProperties: props,
            body: messageBytes);

        return tcs.Task;
    }
}