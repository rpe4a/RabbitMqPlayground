using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace RPCSender // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            var service = new RPCService();
            var model = service.GetRabbitMqConnection().CreateModel();
            // service.SetUpQueue(model);

            await RunOneWayMessageDemo(model, service);
            Console.ReadKey();
        }

        private static async Task RunOneWayMessageDemo(IModel model, RPCService messagingService)
        {
            Console.WriteLine("Enter your message and press Enter. Quit with 'q'.");
            while (true)
            {
                string message = Console.ReadLine();
                if (message.ToLower() == "q") break;

                var response = await messagingService.SendRpcMessageToQueue(message, model, TimeSpan.FromMinutes(1));
                Console.WriteLine($"{response}");
            }
        }
    }
}