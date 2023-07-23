using MassTransit.Core;
using MassTransit.Receiver.Management;

namespace MassTransit.Receiver // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("This is the customer registration event management receiver.");

            IBusControl? bus = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                rabbit.Host(new Uri("rabbitmq://localhost:5672/"), settings =>
                {
                    settings.Password("guest");
                    settings.Username("guest");
                });

                rabbit.ReceiveEndpoint(CustomerRegisteredConsumer.Queue,
                                       conf => { conf.Consumer<CustomerRegisteredConsumer>(); });
            });

            CancellationTokenSource cts = new CancellationTokenSource();

            await bus.StartAsync(cts.Token);
            Console.ReadKey();
            cts.Cancel();
        }

    }
}