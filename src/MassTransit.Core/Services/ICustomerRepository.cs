using MassTransit.Core.Domains;

namespace MassTransit.Core.Services;

public interface ICustomerRepository
{
    void Save(Customer customer);
}