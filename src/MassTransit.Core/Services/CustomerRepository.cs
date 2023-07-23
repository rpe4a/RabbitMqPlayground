using MassTransit.Core.Domains;

namespace MassTransit.Core.Services;

public class CustomerRepository : ICustomerRepository
{
    public void Save(Customer customer)
    {
        Console.WriteLine($"The concrete customer repository was called for customer: {customer}");
    }
}