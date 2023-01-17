namespace Holize.PersistenceFramework;

/// <inheritdoc cref="ISettingsManager" />
public class SettingsManager : ISettingsManager
{
    private readonly List<SettingsPair> _settings = new();

    public void Add<T>(T @default, T current) where T : Settings
    {
        var settingOfTypeT = _settings.SingleOrDefault(x => x.Default.GetType() == typeof(T));
        if (settingOfTypeT is not null)
            throw new ArgumentException($"Setting of the type {typeof(T)} has already been added");

        _settings.Add(SettingsPair.CreateInstance(@default, current));
    }

    public T GetDefault<T>() where T : Settings
    {
        var settings = GetPair<T>().Default;
        return (T)settings;
    }

    public T GetCurrent<T>() where T : Settings
    {
        var settings = GetPair<T>().Current;
        return (T)settings;
    }

    public void Reset<T>() where T : Settings
    {
        var settings = GetPair<T>();
        ResetSettings(settings.Default, settings.Current);
    }


    public void ResetAllSettings()
    {
        foreach (var setting in from setting in _settings let type = setting.Current.GetType() select setting)
            ResetSettings(setting.Default, setting.Current);
    }


    /// <summary>
    ///     Retrieve default and current value pair of the type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Settings" />.</typeparam>
    private SettingsPair GetPair<T>() where T : Settings
    {
        var settings = _settings.Single(x => x.Default.GetType() == typeof(T));
        return settings;
    }


    /// <summary>
    ///     Copies fields from <paramref name="default" /> to <paramref name="current" />.
    /// </summary>
    /// <param name="default">Instance containing default parameters.</param>
    /// <param name="current">Instance whose settings are to be reset.</param>
    private static void ResetSettings(Settings @default, Settings current)
    {
        PropertyHelper.CopyProperties(@default, current);
    }
}