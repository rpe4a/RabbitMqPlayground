namespace MassTransit.Core;

public interface IRegisterDomain
{
    string Target { get; }
    int Importance { get; }
}