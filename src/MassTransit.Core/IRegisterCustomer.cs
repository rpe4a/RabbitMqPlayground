namespace MassTransit.Core;

public class IRegisterCustomer : IRegisterDomain
{
    public const string Queue = "registercustomer.queue";

    public Guid Id { get; init; }
    public DateTime RegisteredUtc { get; init; }
    public int Type { get; init; }
    public string Name { get; init; }
    public bool Preferred { get; init; }
    public decimal DefaultDiscount { get; init; }
    public string Address { get; init; }

    public string Target { get; init; }

    public int Importance { get; init; }
}