# Getting started
Registering simple Settings type.
```csharp
RegisterSettings<T>(this IContainerRegistry containerRegistry, T defaultValue)
```
Example usage:
```csharp
public class ExampleSettings : Settings {}

var defaultSettings = new ExampleSettings();
containerRegistry.RegisterSettings(defaultSettings);
```

# Controlling PersistenceOptions behavior
By default each type of ```Settings``` creates it's own ```PersistenceOptions```.
However it is possible to share the options either by registering ```PersistenceOptions```
or using another overload of ```RegisterSettings<T>```

```csharp
var container = containerRegistry.GetContainer();

containerRegistry.RegisterSingleton<PersistenceOptions>();
var persistenceOptions = container.Resolve<PersistenceOptions>();

// customize PersistenceOptions for all setting except for SettingC
options.AutoSave = true;

containerRegistry.RegisterSettings(new SettingA());
containerRegistry.RegisterSettings(new SettingB());


// provide different options for SettingC
containerRegistry.RegisterSettings(new SettingC(), new PersistenceOptions { AutoSave = false });
```