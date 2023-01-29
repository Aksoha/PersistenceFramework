using Holize.PersistenceFramework;

namespace PersistenceFramework.Extensions.Microsoft.Tests.Models;

public class TestedSettings : Settings
{
    private int _value;

    public int Value
    {
        get => _value;
        set => SetField(ref _value, value);
    }
}