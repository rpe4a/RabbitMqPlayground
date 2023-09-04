using MassTransit.Core;

namespace MassTransit.Publisher // Note: actual namespace depends on the project name.
{
    internal static class Program
    {

        static async Task Main(string[] args)
        {
            Console.Title = "This is the customer registration command publisher.";

            string rabbitMqAddress = "rabbitmq://localhost:5672/";
            Uri rabbitMqRootUri = new Uri(rabbitMqAddress);

            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                rabbit.Host(rabbitMqRootUri, settings =>
                {
                    settings.Password("guest");
                    settings.Username("guest");
                });
            });

            ISendEndpoint sendEndpoint = await
                                             rabbitBusControl.GetSendEndpoint(
                                                 new Uri($"{rabbitMqAddress}/{IRegisterCustomer.Queue}"));

            await sendEndpoint
                .Send<IRegisterCustomer>(new()
                                         {
                                             Address = "New Street",
                                             Id = Guid.NewGuid(),
                                             Preferred = true,
                                             RegisteredUtc = DateTime.UtcNow,
                                             Name = "Nice people LTD",
                                             Type = 1,
                                             DefaultDiscount = 0,
                                             Target = "Customers",
                                             Importance = 1
                                         },
                                         c => c.FaultAddress =
                                                  new Uri(
                                                      "rabbitmq://localhost:5672/accounting/" +
                                                      $"{IRegisterCustomer.Queue}.error.newcustomers"));


            Console.ReadKey();
        }
    }
}