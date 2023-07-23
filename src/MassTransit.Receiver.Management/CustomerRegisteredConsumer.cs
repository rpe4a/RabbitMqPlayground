using MassTransit.Core;

namespace MassTransit.Receiver.Management;

public class CustomerRegisteredConsumer: IConsumer<ICustomerRegistered>
{
    public const string Queue = "customerregistered.menegement.queue";


    public Task Consume(ConsumeContext<ICustomerRegistered> context)
    {
        ICustomerRegistered newCustomer = context.Message;
        Console.WriteLine("A new customer has been registered, congratulations from Management to all parties involved!");
        Console.WriteLine(newCustomer.Address);
        Console.WriteLine(newCustomer.Name);
        Console.WriteLine(newCustomer.Id);
        return Task.FromResult(context.Message);
    }
}