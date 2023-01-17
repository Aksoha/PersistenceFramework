using Holize.PersistenceFramework.Serialization;

namespace Holize.PersistenceFramework.Tests.Models;

[JsonSection("TestSettings:Advanced")]
public class AnnotatedTestSettings : Settings
{
    public string Name { get; set; } = string.Empty;
}