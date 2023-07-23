namespace MassTransit.Core.Domains;

public class Customer
{
    private readonly Guid id;
    private readonly string name;
    private readonly string address;

    public Customer(Guid id, string name, string address)
    {
        this.id = id;
        this.name = name;
        this.address = address;
    }

    public Guid Id => id;

    public string Address => address;

    public string Name => name;

    public DateTime RegisteredUtc { get; set; }
    public int Type { get; set; }
    public bool Preferred { get; set; }
    public decimal DefaultDiscount { get; set; }

    public override string ToString()
    {
        return $"{nameof(id)}: {id}, {nameof(name)}: {name}, {nameof(address)}: {address}";
    }

    public static explicit operator Customer(IRegisterCustomer registerCustomer) =>
        new Customer(registerCustomer.Id, registerCustomer.Name, registerCustomer.Address);
}