using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace HeadersReceiverTwo // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new HeaderService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            service.ReceiveHeaderMessageReceiverTwo(model);
            Console.ReadKey();
        }
    }
}