using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace BadMessageSender // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new BadMessageService();
            var model = service.GetRabbitMqConnection().CreateModel();
            service.SetUpQueue(model);

            RunOneWayMessageDemo(model, service);
            Console.ReadKey();
        }

        private static void RunOneWayMessageDemo(IModel model, BadMessageService messagingService)
        {
            Console.WriteLine("Enter your message and press Enter. Quit with 'q'.");
            while (true)
            {
                string message = Console.ReadLine();
                if (message.ToLower() == "q") break;

                messagingService.SendMessage(message, model);
            }
        }
    }
}