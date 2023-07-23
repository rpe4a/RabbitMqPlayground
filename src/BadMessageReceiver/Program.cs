using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace BadMessageReceiver // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new BadMessageService();
            var model = service.GetRabbitMqConnection().CreateModel();

            ReceiveOneWayMessages(model, service);
            Console.ReadKey();
        }

        private static void ReceiveOneWayMessages(IModel model, BadMessageService service)
        {
            service.ReceiveMessages(model);
        }
    }
}