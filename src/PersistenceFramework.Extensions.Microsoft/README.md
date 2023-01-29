# Getting started
Registering simple Settings type.
```csharp
RegisterSettings<T>(this IServiceCollection services, T defaultValue)
```
Example usage:
```csharp
public class ExampleSettings : Settings {}

var defaultSettings = new ExampleSettings();
services.RegisterSettings(defaultSettings);
```

# Controlling PersistenceOptions behavior
By default each type of ```Settings``` creates it's own ```PersistenceOptions```.
However it is possible to share the options either by registering ```PersistenceOptions```
or using another overload of ```RegisterSettings<T>```

```csharp
IServiceCollection services = new ServiceCollection();

var persistenceOptions = new PersistenceOptions();
services.AddSingleton(persistenceOptions);

// customize PersistenceOptions for all setting except for SettingC
persistenceOptions.AutoSave = true;

services.RegisterSettings(new SettingA());
services.RegisterSettings(new SettingB());


// provide different options for SettingC
services.RegisterSettings(new SettingC(), new PersistenceOptions { AutoSave = false });
```