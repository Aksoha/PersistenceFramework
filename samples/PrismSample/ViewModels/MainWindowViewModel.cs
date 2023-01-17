using System.Windows.Input;
using Holize.PersistenceFramework;
using PrismSample.Settings;

namespace PrismSample.ViewModels;

public class MainWindowViewModel
{
    public MainWindowViewModel(MainWindowSettings settings, ISettingsManager settingsManager)
    {
        Settings = settings;
        SettingsManager = settingsManager;
        ResetSettingsCommand = new ActionCommand(ResetSettings, () => true);
    }

    public MainWindowSettings Settings { get; set; }
    public ICommand ResetSettingsCommand { get; }
    private ISettingsManager SettingsManager { get; }

    private void ResetSettings()
    {
        SettingsManager.Reset<MainWindowSettings>();
    }
}