using MassTransit.Core;

namespace MassTransit.Receiver;

public class RegisterCustomerFaultConsumer: IConsumer<Fault<IRegisterCustomer>>
{

    public Task Consume(ConsumeContext<Fault<IRegisterCustomer>> context)
    {
        Console.WriteLine($"Try to handle {nameof(IRegisterCustomer)} error.");
        IRegisterCustomer originalFault = context.Message.Message;
        ExceptionInfo[] exceptions = context.Message.Exceptions;
        return Task.FromResult(originalFault);
    }
}