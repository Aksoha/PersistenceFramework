using System.Windows;
using Holize.PersistenceFramework;
using Holize.PersistenceFramework.Extensions.Prism;
using Prism.Ioc;
using Prism.Unity;
using PrismSample.Settings;
using PrismSample.Views;

namespace PrismSample;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        var persistenceOptions = new PersistenceOptions();

#if DEBUG
        persistenceOptions.LocalFilesDirectory = ApplicationInfo.ExecutingDirectory;
#endif

        containerRegistry.RegisterInstance(persistenceOptions);
        containerRegistry.RegisterSettings(new MainWindowSettings
        {
            Height = 500,
            Width = 800,
            Left = 0,
            Top = 0,
            WindowState = WindowState.Maximized
        });
    }

    protected override Window CreateShell()
    {
        var window = Container.Resolve<MainWindowView>();
        return window;
    }
}