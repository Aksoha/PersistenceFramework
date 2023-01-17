using System.Reflection;

namespace Holize.PersistenceFramework;

/// <summary>
///     Options of <see cref="PersistenceManager{T}" />.
/// </summary>
public class PersistenceOptions
{
    /// <summary>
    ///     A directory containing settings files.
    /// </summary>
    public string LocalFilesDirectory { get; set; } =
        Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            Assembly.GetCallingAssembly().GetName().Name!);


    /// <summary>
    ///     Indicates whether <see cref="Settings" /> should be automatically saved to the persistant storage.
    /// </summary>
    public bool AutoSave { get; set; } = true;


    /// <summary>
    ///     Indicates whether settings file should be automatically created if the file was not present in the
    ///     <see cref="LocalFilesDirectory" />.
    /// </summary>
    public bool CreateSettingsFile { get; set; } = true;
}