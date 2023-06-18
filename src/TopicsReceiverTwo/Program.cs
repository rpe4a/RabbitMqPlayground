﻿using RabbitMQ.Client;
using RabbitMqPlaygroundLibrary;

namespace TopicsReceiverTwo // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var service = new TopicService();
            IConnection connection = service.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            service.ReceiveTopicsMessageReceiverTwo(model);
            Console.ReadKey();
        }
    }
}