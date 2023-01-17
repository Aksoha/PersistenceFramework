namespace Holize.PersistenceFramework;

/// <summary>
///     Represents a pair of settings.
/// </summary>
/// <seealso cref="ISettingsManager" />
internal class SettingsPair
{
    private SettingsPair(Settings @default, Settings current)
    {
        Default = @default;
        Current = current;
    }

    public Settings Default { get; }
    public Settings Current { get; }


    /// <summary>
    ///     Creates new instance of the <see cref="SettingsPair" />
    /// </summary>
    /// <param name="default">The default value.</param>
    /// <param name="current">The current value.</param>
    /// <typeparam name="T">The type of the <see cref="Settings" />.</typeparam>
    /// <returns>The new instance of the <see cref="SettingsPair" />.</returns>
    public static SettingsPair CreateInstance<T>(T @default, T current) where T : Settings
    {
        return new SettingsPair(@default, current);
    }
}