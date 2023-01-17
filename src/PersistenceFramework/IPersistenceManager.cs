namespace Holize.PersistenceFramework;

/// <summary>
///     Manages storage of <see cref="Settings" />.
/// </summary>
public interface IPersistenceManager<T> where T : Settings
{
    /// <summary>
    ///     Saves <see cref="Settings" /> to a persistant storage.
    /// </summary>
    void Save();

    /// <summary>
    ///     Loads <see cref="Settings" /> from a persistant storage.
    /// </summary>
    void Load();
}