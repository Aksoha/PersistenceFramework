using System.Windows;
using Holize.PersistenceFramework.Serialization;

namespace PrismSample.Settings;

/// <summary>
///     A settings that determines the position on the screen of the main application window.
/// </summary>
[JsonSection("MainWindow")]
public class MainWindowSettings : Holize.PersistenceFramework.Settings
{
    private int _height;
    private int _left;
    private int _top;
    private int _width;
    private WindowState _windowState;


    public int Height
    {
        get => _height;
        set => SetField(ref _height, value);
    }

    public int Width
    {
        get => _width;
        set => SetField(ref _width, value);
    }

    public int Top
    {
        get => _top;
        set => SetField(ref _top, value);
    }

    public int Left
    {
        get => _left;
        set => SetField(ref _left, value);
    }

    public WindowState WindowState
    {
        get => _windowState;
        set => SetField(ref _windowState, value);
    }
}