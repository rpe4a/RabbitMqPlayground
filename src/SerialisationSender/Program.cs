using System.Text;
using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;
using RabbitMqPlaygroundLibrary.Models;

namespace SerialisationSender // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new SerialisationDeserializationService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            service.SetUpQueue(model);
            RunSendMessageDemo(model, service);
            Console.ReadKey();
        }

        private static void RunSendMessageDemo(IModel model, SerialisationDeserializationService service)
        {
            Console.WriteLine("Enter customer name. Quit with 'q'.");
            while (true)
            {
                string customerName = Console.ReadLine();
                if (customerName.ToLower() == "q") break;
                Customer customer = new Customer(customerName);
                service.SendJsonMessage(customer, model);
            }
        }
    }
}