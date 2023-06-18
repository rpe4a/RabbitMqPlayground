using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace HeadersReceiverThree // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new HeaderService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            service.ReceiveHeaderMessageReceiverThree(model);
            Console.ReadKey();
        }
    }
}