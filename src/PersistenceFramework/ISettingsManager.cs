namespace Holize.PersistenceFramework;

/// <summary>
///     Manages instances of default and currently used <see cref="Settings" />.
/// </summary>
public interface ISettingsManager
{
    /// <summary>
    ///     Add information about default parameters for the type <typeparamref name="T" />.
    /// </summary>
    /// <param name="default">An instance containing default parameters.</param>
    /// <param name="current">A currently used instance.</param>
    /// <typeparam name="T">Type of the <see cref="Settings" />.</typeparam>
    /// <exception cref="ArgumentException">Thrown when attempting to add same type more then once.</exception>
    void Add<T>(T @default, T current) where T : Settings;

    /// <summary>
    ///     Retrieve instance of <typeparamref name="T" /> containing default data.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Settings" />.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when type is not found.</exception>
    T GetDefault<T>() where T : Settings;

    /// <summary>
    ///     Retrieve instance of <typeparamref name="T" /> containing current data.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Settings" />.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when type is not found.</exception>
    T GetCurrent<T>() where T : Settings;

    /// <summary>
    ///     Resets the <typeparamref name="T" /> registered with <see cref="SettingsManager.Add{T}" /> to it's default value.
    /// </summary>
    /// <remarks>This operation does not change instance of the object.</remarks>
    /// <typeparam name="T">Type of the <see cref="Settings" />.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown when type is not found.</exception>
    void Reset<T>() where T : Settings;

    /// <summary>
    ///     Resets all the <see cref="Settings" /> registered with <see cref="SettingsManager.Add{T}" /> to theirs default
    ///     values.
    /// </summary>
    /// <remarks>This operation does not change instance of the object.</remarks>
    void ResetAllSettings();
}