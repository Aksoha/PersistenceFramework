using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Holize.PersistenceFramework.Serialization;
using Holize.PersistenceFramework.Tests.Models;

namespace Holize.PersistenceFramework.Tests;

public class PersistenceManagerTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void Save_BasicType(bool autoSave, bool settingIsInFile)
    {
        // arrange
        var settings = new BasicTestSettings
        {
            ValueType = 5,
            ReferenceType = new Person { FirstName = "Wendell", LastName = "Patrick" },
            EnumType = Season.Summer
        };
        settings.Collection.Add("convenience");


        var defaultSettings = new SettingsManager();
        defaultSettings.Add(new BasicTestSettings
        {
            ValueType = int.MaxValue,
            ReferenceType = new Person(),
            EnumType = Season.Winter
        }, settings);


        var fileSystemMock = new MockFileSystem();
        var serializedSetting = settingIsInFile ? Serialize(settings) : "{}";
        const string directory = @"c:\tests";
        var filePath = Path.Combine(directory, "settings.json");
        fileSystemMock.AddFile(filePath, serializedSetting);


        var manager = new PersistenceManager<BasicTestSettings>(defaultSettings, fileSystemMock,
            new PersistenceOptions { LocalFilesDirectory = directory, AutoSave = autoSave });
        manager.Load();

        // Act
        settings.ValueType = 55;
        var person = new Person { FirstName = "Andy", LastName = "Baldwin" };
        settings.ReferenceType = person;
        settings.Collection.Add("smash");
        settings.Collection[0] = "sail";
        settings.EnumType = Season.Autumn;

        if (autoSave is false)
            manager.Save();

        var fileContent = fileSystemMock.File.ReadAllText(filePath);
        var jsonToDeserialize = GetSettingFromJson<BasicTestSettings>(fileContent);
        var settingsFromFile = JsonSerializer.Deserialize<BasicTestSettings>(jsonToDeserialize!)!;


        // assert
        Assert.Equal(settings.ValueType, settingsFromFile.ValueType);
        Assert.Equal(settings.ReferenceType.FirstName, settingsFromFile.ReferenceType.FirstName);
        Assert.Equal(settings.ReferenceType.LastName, settingsFromFile.ReferenceType.LastName);
        Assert.Equal(settings.EnumType, settingsFromFile.EnumType);
        Assert.Equal(settings.Collection.Count, settingsFromFile.Collection.Count);
        foreach (var item in settings.Collection) Assert.Contains(settingsFromFile.Collection, x => x == item);
    }


    [Fact]
    public void Save_AnnotatedType()
    {
        // arrange
        var settings = new AnnotatedTestSettings { Name = "John" };

        var defaultSettings = new SettingsManager();
        defaultSettings.Add(new AnnotatedTestSettings
        {
            Name = "Della"
        }, settings);

        var fileSystemMock = new MockFileSystem();
        const string serializedSetting = "{}";
        var attribute = typeof(AnnotatedTestSettings).GetCustomAttribute<JsonSectionAttribute>();
        const string directory = @"c:tests";
        var filePath = Path.Combine(directory, attribute!.JsonFile);
        fileSystemMock.AddFile(Path.Combine(directory, "settings.json"), serializedSetting);
        fileSystemMock.AddFile(filePath, serializedSetting);

        var manager = new PersistenceManager<AnnotatedTestSettings>(defaultSettings, fileSystemMock,
            new PersistenceOptions { LocalFilesDirectory = directory });
        manager.Save();

        // act
        var fileContent = fileSystemMock.File.ReadAllText(filePath);
        var jsonToDeserialize = GetSettingFromJson<AnnotatedTestSettings>(fileContent);
        var settingsFromFile = JsonSerializer.Deserialize<AnnotatedTestSettings>(jsonToDeserialize!)!;

        // assert
        Assert.Equal(settings.Name, settingsFromFile.Name);
    }


    /// <summary>
    ///     Retrieves a <typeparamref name="T" /> from the <paramref name="json" />.
    /// </summary>
    /// <param name="json">Json in which the search is performed.</param>
    /// <typeparam name="T">The type of <see cref="Settings" />.</typeparam>
    /// <returns>A serialized <typeparamref name="T" /> or <see langword="null" /> when setting was not found.</returns>
    private static string? GetSettingFromJson<T>(string json) where T : Settings
    {
        var sectionName = GetSectionName<T>();
        var jObj = JsonNode.Parse(json);
        var sections = sectionName.Split(":");
        var node = jObj;

        foreach (var section in sections)
        {
            if (node is null)
                return null;
            node = node[section];
        }

        return node?.ToString();
    }


    /// <summary>
    ///     Builds a structure of required <see cref="JsonSectionAttribute.Section">sections</see> and serializes
    ///     <paramref name="value" />.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <typeparam name="T">The type of <see cref="Settings" />.</typeparam>
    /// <returns>
    ///     A json structure containing node and all parent nodes required by the
    ///     <see cref="JsonSectionAttribute.Section">sections</see>.
    /// </returns>
    /// <remarks>This method can be used to add single setting to a mocked file.</remarks>
    private static string Serialize<T>(T value) where T : Settings
    {
        var json = JsonSerializer.Serialize(value);
        var sections = GetSectionName<T>().Split(":");
        return sections.Reverse().Aggregate(json, (current, section) => $"{{\"{section}\":{current}}}");
    }


    /// <summary>
    ///     Get a <see cref="JsonSectionAttribute.Section" /> attached to the <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Settings" />.</typeparam>
    /// <returns>
    ///     A value of <see cref="JsonSectionAttribute" /> or name of the <typeparamref name="T" />
    ///     if attribute wa not present.
    /// </returns>
    private static string GetSectionName<T>() where T : Settings
    {
        var attribute = typeof(T).GetCustomAttribute<JsonSectionAttribute>();
        return attribute is null ? typeof(T).Name : attribute.Section;
    }
}