using RabbitMQ.Client;
using RabbitMqPlayground.Services;

namespace PublishSubscribeReceiverTwo // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new PublishSubscribeService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            service.ReceivePublishSubscribeMessageReceiverTwo(model);
            Console.ReadKey();
        }
    }
}