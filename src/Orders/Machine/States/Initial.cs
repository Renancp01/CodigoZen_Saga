using MassTransit;

namespace Orders.Machine.States;

//Validar como customizar o estado inicial
public class Initial : State
{
    public void Probe(ProbeContext context)
    {
        throw new NotImplementedException();
    }

    public void Accept(StateMachineVisitor visitor)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(State? other)
    {
        throw new NotImplementedException();
    }

    public string Name { get; }
    public Event Enter { get; }
    public Event Leave { get; }
    public Event<State> BeforeEnter { get; }
    public Event<State> AfterLeave { get; }
}