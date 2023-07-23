using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;

namespace OneWayMessagingReciever;

public class OneWayMessageReceiver : DefaultBasicConsumer
{
    private readonly IModel channel;

    public OneWayMessageReceiver(IModel model) : base(model)
    {
        channel = model;
    }


    public override void HandleBasicDeliver(
        string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IBasicProperties properties,
        ReadOnlyMemory<byte> body)
    {
        Console.WriteLine("Message received by the consumer. Check the debug window for details.");
        Debug.WriteLine(string.Concat("Message received from the exchange ", exchange));
        Debug.WriteLine(string.Concat("Content type: ", properties.ContentType));
        Debug.WriteLine(string.Concat("Consumer tag: ", consumerTag));
        Debug.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
        Debug.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(body.Span)));
        Thread.Sleep(500);
        channel.BasicAck(deliveryTag, false);
    }
}