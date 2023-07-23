namespace MassTransit.Core;

public interface ICustomerRegistered
{
    Guid Id { get; set; }
    DateTime RegisteredUtc { get; set; }
    string Name { get; set; }
    string Address { get; set; }
}