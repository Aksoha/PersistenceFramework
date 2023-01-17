using System.IO.Abstractions;
using Prism.Ioc;
using Prism.Unity;
using Unity;

namespace Holize.PersistenceFramework.Extensions.Prism;

public static class RegisterSettingsExtensions
{
    /// <summary>
    ///     Registers <typeparamref name="T" />, <see cref="ISettingsManager" /> and <see cref="IPersistenceManager{T}" /> as a
    ///     singleton.
    /// </summary>
    /// <param name="containerRegistry">The instance of <see cref="IContainerRegistry" />.</param>
    /// <param name="defaultValue">The default value of <typeparamref name="T" /> used by the <see cref="ISettingsManager" />.</param>
    /// <typeparam name="T">The type of <see cref="Settings" /> to register.</typeparam>
    /// <remarks>
    ///     For additional configuration register <see cref="PersistenceOptions" /> before calling this method or use another
    ///     overload.<br />
    ///     This method also registers <see cref="IFileSystem" />.
    /// </remarks>
    public static void RegisterSettings<T>(this IContainerRegistry containerRegistry, T defaultValue) where T : Settings
    {
        var container = containerRegistry.GetContainer();
        if (container.IsRegistered<PersistenceOptions>())
        {
            var options = container.Resolve<PersistenceOptions>();
            RegisterSettings(containerRegistry, defaultValue, options);
            return;
        }

        AddToSettingsManager(containerRegistry, defaultValue);


        containerRegistry.RegisterSingleton<IPersistenceManager<T>, PersistenceManager<T>>();


        // load properties of T instance from the file
        LoadSettingsData<T>(container);
    }


    /// <summary>
    ///     Registers <typeparamref name="T" />, <see cref="ISettingsManager" /> and <see cref="IPersistenceManager{T}" /> as a
    ///     singleton.
    /// </summary>
    /// <param name="containerRegistry">The instance of <see cref="IContainerRegistry" />.</param>
    /// <param name="defaultValue">The default value of <typeparamref name="T" /> used by the <see cref="ISettingsManager" />.</param>
    /// <param name="options">The PersistenceOptions.</param>
    /// <typeparam name="T">The type of <see cref="Settings" /> to register.</typeparam>
    /// <remarks>
    ///     <see cref="PersistenceOptions" /> will not be registered.
    ///     This method also registers <see cref="IFileSystem" />.
    /// </remarks>
    public static void RegisterSettings<T>(this IContainerRegistry containerRegistry, T defaultValue,
        PersistenceOptions options) where T : Settings
    {
        var container = containerRegistry.GetContainer();
        AddToSettingsManager(containerRegistry, defaultValue);

        var settingsManager = container.Resolve<ISettingsManager>();
        var fileManager = container.Resolve<IFileSystem>();
        var persistenceManager = new PersistenceManager<T>(settingsManager, fileManager, options);
        containerRegistry.RegisterInstance<IPersistenceManager<T>>(persistenceManager);

        // load properties of T instance from the file
        LoadSettingsData<T>(container);
    }


    /// <summary>
    ///     Adds a <typeparamref name="T" /> to <see cref="ISettingsManager" />
    /// </summary>
    /// <param name="containerRegistry">The instance of <see cref="IContainerRegistry" />.</param>
    /// <param name="defaultValue">The default value of <typeparamref name="T" />.</param>
    /// <typeparam name="T">The type of <see cref="Settings" />.</typeparam>
    /// <remarks>
    ///     This method registers <see cref="ISettingsManager" />, <typeparamref name="T" /> and <see cref="IFileSystem" /> as
    ///     singleton.
    /// </remarks>
    private static void AddToSettingsManager<T>(IContainerRegistry containerRegistry, T defaultValue) where T : Settings
    {
        var container = containerRegistry.GetContainer();

        // register instance of default settings if it does not exist
        TryRegisterSettingsManager(containerRegistry);

        // register instance of settings and settings manager
        containerRegistry.RegisterSingleton<T>();
        var settingsInstance = container.Resolve<T>();


        // add default values
        var defaultSettings = container.Resolve<ISettingsManager>();
        defaultSettings.Add(defaultValue, settingsInstance);

        containerRegistry.TryRegisterFileSystem();
    }

    /// <summary>
    ///     Registers <see cref="ISettingsManager" /> if it was not registered previously.
    /// </summary>
    /// <param name="containerRegistry">The instance of <see cref="IContainerRegistry" />.</param>
    private static void TryRegisterSettingsManager(IContainerRegistry containerRegistry)
    {
        var container = containerRegistry.GetContainer();
        if (container.IsRegistered<ISettingsManager>()) return;
        container.RegisterSingleton<ISettingsManager, SettingsManager>();
    }


    /// <summary>
    ///     Registers <see cref="IFileSystem" /> if it was not registered previously.
    /// </summary>
    /// <param name="containerRegistry">The instance of <see cref="IContainerRegistry" />.</param>
    private static void TryRegisterFileSystem(this IContainerRegistry containerRegistry)
    {
        var container = containerRegistry.GetContainer();
        if (container.IsRegistered<IFileSystem>()) return;
        containerRegistry.RegisterScoped<IFileSystem, FileSystem>();
    }


    /// <summary>
    ///     Instantiates <typeparamref name="T" /> and loads data from <see cref="IPersistenceManager{T}" />.
    /// </summary>
    /// <param name="container">The instance of <see cref="IUnityContainer" />.</param>
    /// <typeparam name="T">The type of <see cref="Settings" />.</typeparam>
    private static void LoadSettingsData<T>(IUnityContainer container) where T : Settings
    {
        container.Resolve<IPersistenceManager<T>>().Load();
    }
}