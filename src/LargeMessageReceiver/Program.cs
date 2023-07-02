using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace LargeMessageReceiver // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new LargeMessagesService();
            var model = service.GetRabbitMqConnection().CreateModel();

            ReceiveObjectMessages(model, service);
            Console.ReadKey();
        }

        private static void ReceiveObjectMessages(IModel model, LargeMessagesService service)
        {
            service.ReceiveMessages(model);
        }
    }
}