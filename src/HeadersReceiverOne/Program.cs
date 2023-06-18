using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace HeadersReceiverOne // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new HeaderService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            service.ReceiveHeaderMessageReceiverOne(model);
            Console.ReadKey();
        }
    }
}