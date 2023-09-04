using MassTransit.Configuration;
using MassTransit.Core;
using MassTransit.Core.Services;
using MassTransit.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.Receiver // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("This is the customer registration command receiver.");

            var sp = GetServiceProvider();

            IBusControl? bus = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                rabbit.Host(new Uri("rabbitmq://localhost:5672/"), settings =>
                {
                    settings.Password("guest");
                    settings.Username("guest");
                });

                rabbit.ReceiveEndpoint(IRegisterCustomer.Queue,
                                       conf =>
                                       {
                                           conf.Consumer<RegisterCustomerConsumer>(sp);
                                           conf.Consumer<RegisterDomainConsumer>(sp);
                                           conf.UseRetry((retryConfigurator) =>
                                                             retryConfigurator.SetRetryPolicy(
                                                                 (_) => Retry.Incremental(
                                                                     5, TimeSpan.FromSeconds(1),
                                                                     TimeSpan.FromSeconds(1))));
                                       });

                rabbit.ReceiveEndpoint($"{IRegisterCustomer.Queue}.error.newcustomers",
                                       conf => { conf.Consumer<RegisterCustomerFaultConsumer>(sp); });
            });

            CancellationTokenSource cts = new CancellationTokenSource();

            await bus.StartAsync(cts.Token);
            Console.ReadKey();
            cts.Cancel();
        }

        private static ServiceProvider GetServiceProvider()
        {
            ServiceCollection container = new ServiceCollection();

            container.AddSingleton<ICustomerRepository, CustomerRepository>();
            container.AddSingleton<RegisterDomainConsumer>();
            container.AddSingleton<RegisterCustomerConsumer>();
            container.AddSingleton<RegisterCustomerFaultConsumer>();
            container.AddSingleton<ScopedConsumeContextProvider>();

            return container.BuildServiceProvider();
        }

    }
}