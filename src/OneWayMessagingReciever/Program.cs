﻿using System.Text;
using RabbitMQ.Client;
using RabbitMqPlayground.Services;

namespace RabbitMqPlayground // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new OneWayMessageQueueService();
            var model = service.GetRabbitMqConnection().CreateModel();

            ReceiveOneWayMessages(model, service);
            Console.ReadKey();
        }

        private static void ReceiveOneWayMessages(IModel model, OneWayMessageQueueService service)
        {
            service.ReceiveMessages(model);
        }
    }
}