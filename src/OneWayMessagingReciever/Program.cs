using System.Text;
using OneWayMessagingReciever;
using RabbitMQ.Client;
using RabbitMqPlayground.Services;

namespace RabbitMqPlayground // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            ReceiveSingleOneWayMessage();


            // var service = new OneWayMessageQueueService();
            // var model = service.GetRabbitMqConnection().CreateModel();
            //
            // ReceiveOneWayMessages(model, service);
            // Console.ReadKey();
        }

        private static void ReceiveSingleOneWayMessage()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            var connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);
            DefaultBasicConsumer basicConsumer = new OneWayMessageReceiver(channel);
            channel.BasicConsume("OneWayMessageQueue", false, basicConsumer);
            Console.ReadKey();
        }

        private static void ReceiveOneWayMessages(IModel model, OneWayMessageQueueService service)
        {
            service.ReceiveMessages(model);
        }
    }
}