# PersistenceFramework

A framework saves settings when an applications shuts down.
It's able to restore them on the next launch by storing them in JSON file.

# Usage

## Getting started

The base class model for storing data is ```Settings``` which implements ```INotifyPropertyChanged```

```csharp
public class ExampleSettings : Settings
{
    private int _height;
    public int Height
    {
        get => _height;
        set => SetField(ref _height, value); // invoke PropertyChanged
    }
}
```

Loading and saving data is performed by the ```PersistenceManager``` which reads
a [JSON](#controlling-json-file-and-save-behavior) file.

```csharp
var settings = new ExampleSettings { Height = 185 };
var settingsManager = new SettingsManager();
settingsManager.Add(new ExampleSettings { Height = 168 }, settings);
var persistenceManager = new PersistenceManager<ExampleSettings>(settingsManager, new FileSystem());

persistenceManager.Load();
settings.Height = 190; // setting will be auto saved
```

## Controlling json file and save behavior

By default settings are saved automatically in the ```settings.json``` file located in the
```%LOCALAPPDATA%\{AssemblyName}```
However this behavior can by customized with ```PersistenceOptions```.

⚠️ ```PersistenceOptions.LocalFilesDirectory``` only controls the directory of JSON files but not the names.
For additional information check [this](#Class-serialization).

```csharp
    new PersistenceManager<ExampleSettings>(settingsManager, fileSystem,
        new PersistenceOptions { LocalFilesDirectory = @"C:\Users\user\AppData\Roaming", AutoSave = false });
```

## Class serialization

Serialization can be controlled with attributes provided by ```System.Text.Json.Serialization```.
```JsonSectionAttribute``` defines a name for the section in which the setting is serialized as well as file name in
which setting is stored.

By default settings are wrapped in an object. The name of the wrapped object is the name as the name of the settings
class.

```json
{
  "ExampleSettings": {
    "Height": 185
  }
}
```

This behavior can be overwritten by specifying ```Section``` from the ```JsonSectionAttribute``` on the class.

```csharp
[JsonSection("Shared:Theme")]
public class ThemeSettings : Settings {}
```

The following class will be serialized as shown below:

```json
{
  "Shared": {
    "Theme": {
    }
  }
}
```

## Settings manager

The library provides a settings manager which can store all of the application settings
as well as reset them to the default value.

Resetting the settings can be performed by calling ```SettingsManager.Reset<T>``` or ```SeeeingsManager.ResetAll```.
Since the main usage of this library is an UI framework, it is important to keep references of the objects.
Calling those methods will not modify any references of "current" object registered by
```SettingsManager.Add<T>(@default, current)```. Resetting will be performed by modifying value types (including nested
types).

⚠️ Resetting ```ICollection<T>``` will copy references from the defaultSettings instead of updating them.