namespace MassTransit.Core;

public class IRegisterCustomer
{
    public const string Queue = "registercustomer.queue";
    
    public Guid Id { get; set; }
    public DateTime RegisteredUtc { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
    public bool Preferred { get; set; }
    public decimal DefaultDiscount { get; set; }
    public string Address { get; set; }
}