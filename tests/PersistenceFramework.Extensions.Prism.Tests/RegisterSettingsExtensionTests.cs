using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Holize.PersistenceFramework.Extensions.Prism.Tests.Models;
using Prism.Ioc;
using Prism.Unity;
using Unity;

namespace Holize.PersistenceFramework.Extensions.Prism.Tests;

public class RegisterSettingsExtensionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RegisterSettings(bool useDefaultOptions)
    {
        // arrange
        IContainerRegistry containerRegistry = new UnityContainerExtension();
        var container = containerRegistry.GetContainer();

        var fileSystemMock = new MockFileSystem();
        const string directory = @"c:\tests";
        var filePath = Path.Combine(directory, "settings.json");
        fileSystemMock.AddFile(filePath, "{}");

        containerRegistry.RegisterInstance<IFileSystem>(fileSystemMock);
        containerRegistry.RegisterInstance(fileSystemMock.File);

        if (useDefaultOptions is false)
        {
            containerRegistry.RegisterSingleton<PersistenceOptions>();
            var persistenceOptions = container.Resolve<PersistenceOptions>();
            persistenceOptions.LocalFilesDirectory = directory;
        }

        var defaultSetting = new TestedSettings();

        // act
        containerRegistry.RegisterSettings(defaultSetting);
        var act = container.Resolve<TestedSettings>();


        // assert
        Assert.NotNull(act);
        Assert.Same(act, container.Resolve<TestedSettings>());
    }
}