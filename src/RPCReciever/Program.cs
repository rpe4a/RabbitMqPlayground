using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace RPCReciever // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new RPCService();
            var model = service.GetRabbitMqConnection().CreateModel();

            service.ReceiveMessages(model);
            Console.ReadKey();
        }
    }
}