using RabbitMQ.Client;
using RabbitMqPlayground.Services;

namespace PublishSubscribeSender // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new PublishSubscribeService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            // service.SetUpQueue(model);
            RunPublishSubscribeMessageDemo(model, service);
            Console.ReadKey();
        }

        private static void RunPublishSubscribeMessageDemo(IModel model, PublishSubscribeService service)
        {
            while (true)
            {
                string message = Guid.NewGuid().ToString();
                service.SendMessageToPublishSubscribeQueues(message, model);
                Thread.Sleep(Random.Shared.Next(500));
            }
        }
    }
}