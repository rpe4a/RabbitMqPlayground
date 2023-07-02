using System.IO.Compression;
using System.Net.Mime;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPlaygroundLibrary;

public class SerialisationDeserializationService
{
    private const string _hostName = "localhost";
    private const string _userName = "guest";
    private const string _password = "guest";
    private const string _queue = "SerialisationDeserializationQueue";
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

    public void SendJsonMessage<T>(T data, IModel model)
    {
        var basicProperties = model.CreateBasicProperties();
        basicProperties.Persistent = true;
        basicProperties.ContentType = MediaTypeNames.Application.Json;
        basicProperties.Type = typeof(T).ToString();
        basicProperties.ContentEncoding = "gzip";

        var bytes = JsonSerializer.SerializeToUtf8Bytes(data);
        using var to = new MemoryStream();
        using var gZipStream = new GZipStream(to, CompressionMode.Compress);
        gZipStream.Write(bytes, 0, bytes.Length);
        gZipStream.Flush();

        model.BasicPublish("", _queue, basicProperties, to.ToArray());
    }

    public void ReceiveMessages(IModel model)
    {
        model.BasicQos(0, 1, false);
        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
        consumer.Received += (consumer, @event) =>
        {
            if (@event.BasicProperties.IsContentTypePresent()
                && @event.BasicProperties.IsTypePresent()
                && string.Equals(
                    MediaTypeNames.Application.Json,
                    @event.BasicProperties.ContentType,
                    StringComparison.OrdinalIgnoreCase)
                && string.Equals(
                    "gzip",
                    @event.BasicProperties.ContentEncoding,
                    StringComparison.OrdinalIgnoreCase)
               )
            {
                var body = @event.Body;
                
                using var from = new MemoryStream(body.ToArray());
                using var to = new MemoryStream();
                using var gZipStream = new GZipStream(from, CompressionMode.Decompress);
                gZipStream.CopyTo(to);
                
                var type = @event.BasicProperties.Type;
                var message = JsonSerializer.Deserialize(to.ToArray(), Type.GetType(type));

                Console.WriteLine(message);
            }


            model.BasicAck(@event.DeliveryTag, false);
        };

        model.BasicConsume(_queue, false, consumer);
    }
}