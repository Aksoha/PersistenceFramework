using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Holize.PersistenceFramework.Extensions.Microsoft;

public static class RegisterSettingsExtensions
{
    /// <summary>
    ///     A collection containing all registered <see cref="IPersistenceManager{T}" /> types.
    /// </summary>
    private static readonly List<Type> Registrations = new();


    /// <summary>
    ///     Registers <typeparamref name="T" />, <see cref="ISettingsManager" /> and <see cref="IPersistenceManager{T}" /> as a
    ///     singleton.
    /// </summary>
    /// <param name="services">The instance of <see cref="IServiceCollection" />.</param>
    /// <param name="defaultValue">The default value of <typeparamref name="T" /> used by the <see cref="ISettingsManager" />.</param>
    /// <typeparam name="T">The type of <see cref="Settings" /> to register.</typeparam>
    /// <remarks>
    ///     For additional configuration register <see cref="PersistenceOptions" /> before calling this method or use another
    ///     overload.<br />
    ///     This method also registers <see cref="IFileSystem" />.
    /// </remarks>
    public static void RegisterSettings<T>(this IServiceCollection services, T defaultValue)
        where T : Settings
    {
        services.RegisterDependencies<T>();
        services.TryAddSingleton<PersistenceOptions>();

        Registrations.Add(typeof(IPersistenceManager<T>));

        services.AddSingleton<IPersistenceManager<T>>(sp =>
        {
            var settingsManager = sp.GetRequiredService<ISettingsManager>();
            var fileManager = sp.GetRequiredService<IFileSystem>();
            var currentSettings = sp.GetRequiredService<T>();
            var options = sp.GetRequiredService<PersistenceOptions>();

            settingsManager.Add(defaultValue, currentSettings);

            var output = new PersistenceManager<T>(settingsManager, fileManager, options);
            output.Load();
            return output;
        });
    }


    /// <summary>
    ///     Registers <typeparamref name="T" />, <see cref="ISettingsManager" /> and <see cref="IPersistenceManager{T}" /> as a
    ///     singleton.
    /// </summary>
    /// <param name="services">The instance of <see cref="IServiceCollection" />.</param>
    /// <param name="defaultValue">The default value of <typeparamref name="T" /> used by the <see cref="ISettingsManager" />.</param>
    /// <param name="options">The PersistenceOptions.</param>
    /// <typeparam name="T">The type of <see cref="Settings" /> to register.</typeparam>
    /// <remarks>
    ///     <see cref="PersistenceOptions" /> will not be registered.
    ///     This method also registers <see cref="IFileSystem" />.
    /// </remarks>
    public static void RegisterSettings<T>(this IServiceCollection services, T defaultValue, PersistenceOptions options)
        where T : Settings
    {
        services.RegisterDependencies<T>();

        Registrations.Add(typeof(IPersistenceManager<T>));

        services.AddSingleton<IPersistenceManager<T>>(sp =>
        {
            var settingsManager = sp.GetRequiredService<ISettingsManager>();
            var fileManager = sp.GetRequiredService<IFileSystem>();
            var currentSettings = sp.GetRequiredService<T>();

            settingsManager.Add(defaultValue, currentSettings);

            var output = new PersistenceManager<T>(settingsManager, fileManager, options);
            output.Load();
            return output;
        });
    }


    /// <summary>
    ///     Sets properties for all registered <see cref="Settings" />.
    /// </summary>
    /// <param name="host">The host.</param>
    public static void InitializeSettings(this IHost host)
    {
        foreach (var registration in Registrations) host.Services.GetService(registration);

        Registrations.Clear();
    }


    /// <summary>
    ///     Registers <see cref="IFileSystem" />, <see cref="ISettingsManager" />, <see cref="Settings" /> as singleton.
    /// </summary>
    /// <param name="services">The instance of <see cref="IServiceCollection" />.</param>
    /// <typeparam name="T">The type of <see cref="Settings" /> to register.</typeparam>
    private static void RegisterDependencies<T>(this IServiceCollection services) where T : Settings
    {
        services.TryAddTransient<IFileSystem, FileSystem>();
        services.TryAddSingleton<ISettingsManager, SettingsManager>();
        services.AddSingleton<T>();
    }
}