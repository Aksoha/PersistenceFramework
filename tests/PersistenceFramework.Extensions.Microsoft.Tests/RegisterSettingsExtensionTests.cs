using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Holize.PersistenceFramework;
using Holize.PersistenceFramework.Extensions.Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using PersistenceFramework.Extensions.Microsoft.Tests.Models;

namespace PersistenceFramework.Extensions.Microsoft.Tests;

public class RegisterSettingsExtensionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RegisterSettings(bool useOptions)
    {
        // arrange
        IServiceCollection services = new ServiceCollection();
        var expectedValue = 15;

        var fileSystemMock = new MockFileSystem();
        const string directory = @"c:\tests";
        var filePath = Path.Combine(directory, "settings.json");
        fileSystemMock.AddFile(filePath, MockSettingInFile(expectedValue));

        services.AddSingleton<IFileSystem>(fileSystemMock);
        services.AddSingleton(fileSystemMock.File);

        var defaultSettings = new TestedSettings { Value = 5 };


        // act
        if (useOptions)
        {
            var persistenceOptions = new PersistenceOptions();
            services.AddSingleton(persistenceOptions);
            persistenceOptions.LocalFilesDirectory = directory;
            services.RegisterSettings(defaultSettings, persistenceOptions);
        }
        else
        {
            services.RegisterSettings(defaultSettings);
        }

        var sp = services.BuildServiceProvider();
        var hostMock = new Mock<IHost>();
        hostMock.Setup(x => x.Services).Returns(sp);
        hostMock.Object.InitializeSettings();


        // assert
        AssertLifetime(services, typeof(ISettingsManager), ServiceLifetime.Singleton);
        AssertLifetime(services, typeof(PersistenceOptions), ServiceLifetime.Singleton);
        AssertLifetime(services, typeof(TestedSettings), ServiceLifetime.Singleton);
        AssertLifetime(services, typeof(IPersistenceManager<TestedSettings>), ServiceLifetime.Singleton);
        AssertLifetime(services, typeof(TestedSettings), ServiceLifetime.Singleton);

        var actual = sp.GetService<TestedSettings>()!;
        if (useOptions) Assert.True(expectedValue == actual.Value, "value was not loaded from the file");
    }

    private static void AssertLifetime(IServiceCollection services, Type type, ServiceLifetime lifetime)
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == type);
        if (descriptor is null)
            throw new AssertLifetimeException(services, type, null, lifetime);

        if (descriptor.Lifetime != lifetime)
            throw new AssertLifetimeException(services, type, descriptor.Lifetime, lifetime);
    }


    /// <summary>
    ///     Creates a mock of json file which contains <see cref="TestedSettings" />.
    /// </summary>
    /// <param name="value">The value of <see cref="TestedSettings" /> property.</param>
    private static MockFileData MockSettingInFile(int value)
    {
        var settings = new TestedSettings { Value = value };
        var settingsAsString = JsonSerializer.Serialize(settings);
        var output = $"{{\"{nameof(TestedSettings)}\":{settingsAsString}}}";
        return output;
    }
}