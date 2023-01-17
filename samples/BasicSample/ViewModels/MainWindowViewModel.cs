using System.IO.Abstractions;
using System.Windows;
using System.Windows.Input;
using BasicSample.Settings;
using Holize.PersistenceFramework;

namespace BasicSample.ViewModels;

public class MainWindowViewModel
{
    private readonly ISettingsManager _settingsManager;

    public MainWindowViewModel()
    {
        var defaultSettings = new MainWindowSettings
        {
            Height = 500,
            Width = 800,
            Left = 0,
            Top = 0,
            WindowState = WindowState.Maximized
        };

        _settingsManager = new SettingsManager();
        _settingsManager.Add(defaultSettings, Settings);

        var fileSystem = new FileSystem();
        var persistenceOptions = new PersistenceOptions();

#if DEBUG
        persistenceOptions.LocalFilesDirectory = ApplicationInfo.ExecutingDirectory;
#endif

        var persistenceManager = new PersistenceManager<MainWindowSettings>(_settingsManager, fileSystem,
            persistenceOptions);
        persistenceManager.Load();


        ResetSettingsCommand = new ActionCommand(ResetSettings, () => true);
    }

    public MainWindowSettings Settings { get; } = new();

    public ICommand ResetSettingsCommand { get; }

    private void ResetSettings()
    {
        _settingsManager.Reset<MainWindowSettings>();
    }
}