using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace ScatterGatherSender // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new ScatterGatherService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            // service.SetUpQueue(model);
            RunPublishSubscribeMessageDemo(model, service);
            Console.ReadKey();
        }

        private static void RunPublishSubscribeMessageDemo(IModel model, ScatterGatherService service)
        {
            Console.WriteLine("Enter your message as follows: the routing key, followed by a semicolon, and then the message. Quit with 'q'.");
            while (true)
            {
                string fullEntry = Console.ReadLine();
                string[] parts = fullEntry.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string key = parts[0];
                string message = parts[1];
                if (message.ToLower() == "q") break;
                // service.SendScatterGatherMessage(message, key, model);
            }
        }
    }
}