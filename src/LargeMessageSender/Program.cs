using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;
using RabbitMqPlaygroundLibrary.Models;

namespace LargeMessageSender // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new LargeMessagesService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            service.SetUpQueue(model);
            RunSendMessageDemo(model, service);
            Console.ReadKey();
        }

        private static void RunSendMessageDemo(IModel model, LargeMessagesService service)
        {
            string filePath = @"C:\Users\zapma\Downloads\import_data.752Dn000002mAgr.csv";
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (true)
            {
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    service.SendLargeMessage(File.ReadAllBytes(filePath), model);
                }

                keyInfo = Console.ReadKey();
            }
        }
    }
}