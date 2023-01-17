namespace Holize.PersistenceFramework.Serialization;

/// <summary>
///     Represents a search path for object contained in json file.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class JsonSectionAttribute : Attribute
{
    public JsonSectionAttribute(string section, string jsonFile = "settings.json")
    {
        Section = section;
        JsonFile = jsonFile;
    }

    /// <summary>
    ///     Section in which object is contained.
    ///     <example>For object MainWindow contained in "Settings" session will look as fallows: "Settings:MainWindow".</example>
    /// </summary>
    public string Section { get; set; }

    /// <summary>
    ///     Relative path to json file.
    /// </summary>
    public string JsonFile { get; set; }
}