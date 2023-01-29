using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace PersistenceFramework.Extensions.Microsoft.Tests;

public class AssertLifetimeException : XunitException
{
    public AssertLifetimeException(IServiceCollection serviceCollection, Type type, ServiceLifetime? actual,
        ServiceLifetime expected)
        : base("Assert.Lifetime() Failure")
    {
        ServiceCollection = serviceCollection;
        Type = type;
        Actual = actual;
        Expected = expected;
    }

    public IServiceCollection ServiceCollection { get; }
    public Type Type { get; }
    public ServiceLifetime? Actual { get; }
    public ServiceLifetime Expected { get; }

    public override string Message => FormatException();

    private string FormatException()
    {
        return Actual is null
            ? $"Service of a type {Type} was not not registered in a service collection"
            : $"Expected lifetime for a type {Type} was {Expected}, actual lifetime is {Actual}";
    }
}