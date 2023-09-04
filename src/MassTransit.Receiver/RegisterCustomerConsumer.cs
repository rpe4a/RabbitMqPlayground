using MassTransit.Core;
using MassTransit.Core.Domains;
using MassTransit.Core.Services;

namespace MassTransit.Receiver;

public class RegisterCustomerConsumer : IConsumer<IRegisterCustomer>
{
    private readonly ICustomerRepository repository;

    public RegisterCustomerConsumer(ICustomerRepository repository)
    {
        this.repository = repository ?? throw new ArgumentNullException();
    }
    
    public Task Consume(ConsumeContext<IRegisterCustomer> context)
    {
        Console.WriteLine("A new customer has signed up, it's time to register it. Details: ");
        IRegisterCustomer newCustomer = context.Message;
        repository.Save((Customer)newCustomer);
        return Task.FromResult(context.Message);
    }
}