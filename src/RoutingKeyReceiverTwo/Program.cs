using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace RoutingKeyReceiverTwo // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new RoutingKeyService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            service.ReceiveRoutingKeyMessageReceiverTwo(model);
            Console.ReadKey();
        }
    }
}