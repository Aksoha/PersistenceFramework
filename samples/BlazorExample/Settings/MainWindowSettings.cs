using Holize.PersistenceFramework.Serialization;

namespace BlazorExample.Settings;

/// <summary>
///     A settings that determines the position on the screen of the main application window.
/// </summary>
[JsonSection("Counter")]
public class CounterSettings : Holize.PersistenceFramework.Settings
{
    private int _current;
    private int _increment;

    public int Current
    {
        get => _current;
        set => SetField(ref _current, value);
    }

    public int Increment
    {
        get => _increment;
        set => SetField(ref _increment, value);
    }
}