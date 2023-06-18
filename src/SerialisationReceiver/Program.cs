using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace SerialisationReceiver // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new SerialisationDeserializationService();
            var model = service.GetRabbitMqConnection().CreateModel();

            ReceiveObjectMessages(model, service);
            Console.ReadKey();
        }

        private static void ReceiveObjectMessages(IModel model, SerialisationDeserializationService service)
        {
            service.ReceiveMessages(model);
        }
    }
}