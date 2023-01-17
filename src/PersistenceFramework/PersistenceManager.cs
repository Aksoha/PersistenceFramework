using System.ComponentModel;
using System.IO.Abstractions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Holize.PersistenceFramework.Serialization;

namespace Holize.PersistenceFramework;

/// <summary>
///     Manages storage of the <see cref="Settings" />.
/// </summary>
/// <typeparam name="T">The type of <see cref="Settings" />.</typeparam>
public class PersistenceManager<T> : IPersistenceManager<T> where T : Settings
{
    /// <summary>
    ///     Url of the file containing <typeparamref name="T" />.
    /// </summary>
    private readonly string _filePath;

    /// <summary>
    ///     Name of the json object in which <typeparamref name="T" /> is contained.
    /// </summary>
    /// <seealso cref="JsonSectionAttribute" />
    private readonly string _sectionName;

    private readonly ISettingsManager _settingsManager;

    /// <summary>
    ///     Current value of the <typeparamref name="T" />.
    /// </summary>
    private readonly T _value;


    /// <summary>
    ///     Creates new instance of the <see cref="PersistenceManager{T}" /> with default options.
    /// </summary>
    /// <param name="settingsManager">The settings manager.</param>
    /// <param name="fileManager">The file manager.</param>
    public PersistenceManager(ISettingsManager settingsManager, IFileSystem fileManager)
        : this(settingsManager, fileManager, new PersistenceOptions())
    {
    }


    /// <summary>
    ///     Creates new instance of the <see cref="PersistenceManager{T}" />.
    /// </summary>
    /// <param name="settingsManager">The settings manager.</param>
    /// <param name="fileManager">The file manager.</param>
    /// <param name="options">The persistence options.</param>
    public PersistenceManager(ISettingsManager settingsManager, IFileSystem fileManager, PersistenceOptions options)
    {
        _settingsManager = settingsManager;
        FileSystem = fileManager;
        Options = options;
        _value = _settingsManager.GetCurrent<T>();


        // get section and file name
        var attribute = typeof(T).GetCustomAttribute<JsonSectionAttribute>() ??
                        new JsonSectionAttribute(typeof(T).Name);
        _sectionName = attribute.Section;
        _filePath = PathManager.Combine(options.LocalFilesDirectory, attribute.JsonFile);
    }

    private IFileSystem FileSystem { get; }
    private IFile FileManager => FileSystem.File;
    private IDirectory DirectoryManager => FileSystem.Directory;
    private IPath PathManager => FileSystem.Path;

    private PersistenceOptions Options { get; }

    public void Save()
    {
        var jsonStr = FileManager.ReadAllText(_filePath);
        var jsonNode = JsonNode.Parse(jsonStr)!;

        var sections = GetSections();
        var serializedValue = JsonSerializer.Serialize(_value);

        // passing value as a node instead of a string due to a problem which
        // caused string to be serialized again when performing assignment to JsonNode in the UpdateOrCreateJsonSection
        var valueAsNode = JsonNode.Parse(serializedValue)!;
        UpdateOrCreateJsonSection(jsonNode, sections, valueAsNode);

        var output = jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        FileManager.WriteAllText(_filePath, output);
    }


    public void Load()
    {
        if (FileManager.Exists(_filePath) is false) CreateFile();
        var jsonStr = FileManager.ReadAllText(_filePath);
        var jsonObj = JsonNode.Parse(jsonStr)!.AsObject();

        // get a part of json file where object is stored
        var section = GetSectionContainingValue(jsonObj);


        // convert json to settings object
        var settings = section.Deserialize<T>() ?? _settingsManager.GetDefault<T>();

        PropertyHelper.CopyProperties(settings, _value);

        if (Options.AutoSave) _value.PropertyChanged += Save;
    }

    private void Save(object? sender, PropertyChangedEventArgs e)
    {
        Save();
    }


    /// <summary>
    ///     Retrieves a <see cref="JsonNode" /> that contains a <typeparamref name="T" />.
    /// </summary>
    /// <param name="node">Node in which search is performed.</param>
    /// <returns></returns>
    private JsonNode? GetSectionContainingValue(JsonNode node)
    {
        var sections = GetSections();
        var currentChild = node;

        foreach (var section in sections)
        {
            if (currentChild is null)
                return null;
            currentChild = currentChild[section];
        }

        return currentChild;
    }


    /// <summary>
    ///     Retrieves a list containing hierarchy of the <typeparamref name="T" /> in the json file.
    /// </summary>
    /// <seealso cref="JsonSectionAttribute" />
    private IReadOnlyList<string> GetSections()
    {
        return _sectionName.Split(":");
    }

    /// <summary>
    ///     Retrieves a <see cref="JsonNode" /> by the key.
    /// </summary>
    /// <param name="node">Node in which the search is performed.</param>
    /// <param name="key">Name of the section.</param>
    /// <remarks>This method does not scan parent/children keys.</remarks>
    private static JsonNode? GetNode(JsonNode node, string key)
    {
        return node[key];
    }

    /// <summary>
    ///     Performs an update on the object specified by the <paramref name="sections">section path</paramref>.
    ///     If the path is not present in the json file it will be created.
    /// </summary>
    /// <param name="node">Node which will be updated.</param>
    /// <param name="sections">The path to the object.</param>
    /// <param name="newValue">New value.</param>
    private static void UpdateOrCreateJsonSection(JsonNode node, IReadOnlyList<string> sections, JsonNode newValue)
    {
        var i = 0;
        var currentChild = node;


        // find nested object
        while (i < sections.Count - 1)
        {
            var currentSection = sections[i];
            var previousChild = currentChild;
            currentChild = GetNode(currentChild, currentSection);

            // section not found, create new section
            if (currentChild is null)
            {
                previousChild[currentSection] = new JsonObject();
                currentChild = previousChild[currentSection]!;
            }

            i++;
        }

        // update json structure
        currentChild[sections[^1]] = newValue;
    }

    /// <summary>
    ///     Creates empty json file in the <see cref="_filePath" />.
    /// </summary>
    private void CreateFile()
    {
        DirectoryManager.CreateDirectory(PathManager.GetDirectoryName(_filePath)!);
        using var fileStream = FileManager.Create(_filePath);
        const string data = "{}";
        var info = new UTF8Encoding(true).GetBytes(data);
        fileStream.Write(info, 0, info.Length);
    }
}