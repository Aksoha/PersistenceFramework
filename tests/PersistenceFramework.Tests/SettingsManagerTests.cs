using Holize.PersistenceFramework.Tests.Models;

namespace Holize.PersistenceFramework.Tests;

public class SettingsManagerTests
{
    [Fact]
    public void ResetAllSettings()
    {
        // arrange
        var defaultSettings1 = new BasicTestSettings();
        defaultSettings1.Collection.Add("height");
        defaultSettings1.Collection.Add("perception");
        defaultSettings1.ReferenceType.FirstName = "Joy";
        defaultSettings1.ReferenceType.LastName = "Holt";
        defaultSettings1.ValueType = 15;
        defaultSettings1.EnumType = Season.Summer;

        var currentSettings1 = new BasicTestSettings();
        currentSettings1.Collection.Add("choose");
        currentSettings1.ReferenceType.FirstName = "Ted";
        currentSettings1.ReferenceType.LastName = "Black";
        currentSettings1.ValueType = 368;
        currentSettings1.EnumType = Season.Winter;

        var collectionOfCurrentSettings1 = currentSettings1.Collection;
        var referenceTypeOfCurrentSettings1 = currentSettings1.ReferenceType;


        var defaultSettings2 = new AnnotatedTestSettings();
        defaultSettings2.Name = "Virgil";

        var currentSettings2 = new AnnotatedTestSettings();
        currentSettings2.Name = "Florence";

        var settingsManager = new SettingsManager();
        settingsManager.Add(defaultSettings1, currentSettings1);
        settingsManager.Add(defaultSettings2, currentSettings2);

        // act
        settingsManager.ResetAllSettings();


        // assert
        Assert.NotSame(defaultSettings1, currentSettings1);
        Assert.NotSame(defaultSettings2, currentSettings2);

        Assert.Same(collectionOfCurrentSettings1, currentSettings1.Collection);
        Assert.Same(referenceTypeOfCurrentSettings1, currentSettings1.ReferenceType);

        Assert.Equal(defaultSettings1.ValueType, currentSettings1.ValueType);
        Assert.Equal(defaultSettings1.ReferenceType.FirstName, currentSettings1.ReferenceType.FirstName);
        Assert.Equal(defaultSettings1.ReferenceType.LastName, currentSettings1.ReferenceType.LastName);
        Assert.Equal(defaultSettings1.EnumType, currentSettings1.EnumType);
        Assert.Equal(defaultSettings1.Collection.Count, currentSettings1.Collection.Count);
        foreach (var item in defaultSettings1.Collection) Assert.Contains(currentSettings1.Collection, x => x == item);

        Assert.Equal(defaultSettings2.Name, currentSettings2.Name);
    }

    [Fact]
    public void Reset_WhenSettingIsPresent()
    {
        // arrange
        var defaultSettings1 = new BasicTestSettings();
        defaultSettings1.Collection.Add("height");
        defaultSettings1.Collection.Add("perception");
        defaultSettings1.ReferenceType.FirstName = "Joy";
        defaultSettings1.ReferenceType.LastName = "Holt";
        defaultSettings1.ValueType = 15;
        defaultSettings1.EnumType = Season.Summer;

        var currentSettings1 = new BasicTestSettings();
        currentSettings1.Collection.Add("choose");
        currentSettings1.ReferenceType.FirstName = "Ted";
        currentSettings1.ReferenceType.LastName = "Black";
        currentSettings1.ValueType = 368;
        defaultSettings1.EnumType = Season.Winter;

        var collectionOfCurrentSettings1 = currentSettings1.Collection;
        var referenceTypeOfCurrentSettings1 = currentSettings1.ReferenceType;


        var defaultSettings2 = new AnnotatedTestSettings();
        defaultSettings2.Name = "Virgil";

        var currentSettings2 = new AnnotatedTestSettings();
        const string currentSettings2Name = "Florence";
        currentSettings2.Name = currentSettings2Name;

        var settingsManager = new SettingsManager();
        settingsManager.Add(defaultSettings1, currentSettings1);
        settingsManager.Add(defaultSettings2, currentSettings2);

        // act
        settingsManager.Reset<BasicTestSettings>();


        // assert
        Assert.NotSame(defaultSettings1, currentSettings1);
        Assert.NotSame(defaultSettings2, currentSettings2);

        Assert.Same(collectionOfCurrentSettings1, currentSettings1.Collection);
        Assert.Same(referenceTypeOfCurrentSettings1, currentSettings1.ReferenceType);

        Assert.Equal(defaultSettings1.ValueType, currentSettings1.ValueType);
        Assert.Equal(defaultSettings1.ReferenceType.FirstName, currentSettings1.ReferenceType.FirstName);
        Assert.Equal(defaultSettings1.ReferenceType.LastName, currentSettings1.ReferenceType.LastName);
        Assert.Equal(defaultSettings1.Collection.Count, currentSettings1.Collection.Count);
        Assert.Equal(defaultSettings1.EnumType, currentSettings1.EnumType);
        foreach (var item in defaultSettings1.Collection) Assert.Contains(currentSettings1.Collection, x => x == item);

        Assert.Equal(currentSettings2Name, currentSettings2.Name);
    }

    [Fact]
    public void Reset_whenSettingIsNotPresent_ThrowsInvalidOperationException()
    {
        // arrange
        var settingsManager = new SettingsManager();

        // act
        void Act()
        {
            settingsManager.Reset<AnnotatedTestSettings>();
        }

        // assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void GetDefault_WhenSettingIsPresent()
    {
        // arrange
        var defaultSettings = new BasicTestSettings();
        var currentSettings = new BasicTestSettings();
        var settingsManager = new SettingsManager();
        settingsManager.Add(defaultSettings, currentSettings);

        // act
        var act = settingsManager.GetDefault<BasicTestSettings>();

        // assert
        Assert.Same(defaultSettings, act);
    }

    [Fact]
    public void GetDefault_WhenSettingIsNotPresent_ThrowsInvalidOperationException()
    {
        // arrange
        var settingsManager = new SettingsManager();

        // act
        AnnotatedTestSettings Act()
        {
            return settingsManager.GetDefault<AnnotatedTestSettings>();
        }

        // assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void GetCurrent_WhenSettingIsPresent()
    {
        // arrange
        var defaultSettings = new BasicTestSettings();
        var currentSettings = new BasicTestSettings();
        var settingsManager = new SettingsManager();
        settingsManager.Add(defaultSettings, currentSettings);

        // act
        var act = settingsManager.GetCurrent<BasicTestSettings>();

        // assert
        Assert.Same(currentSettings, act);
    }

    [Fact]
    public void GetCurrent_WhenSettingIsNotPresent_ThrowsInvalidOperationException()
    {
        // arrange
        var settingsManager = new SettingsManager();

        // act
        AnnotatedTestSettings Act()
        {
            return settingsManager.GetCurrent<AnnotatedTestSettings>();
        }

        // assert
        Assert.Throws<InvalidOperationException>(Act);
    }


    [Fact]
    public void Add_Duplicate_ThrowsArgumentException()
    {
        // arrange
        var defaultSettings = new BasicTestSettings();
        var currentSettings = new BasicTestSettings();

        var settingsManager = new SettingsManager();
        settingsManager.Add(defaultSettings, currentSettings);

        // act
        void Act()
        {
            settingsManager.Add(defaultSettings, currentSettings);
        }


        // assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Reset_RisesPropertyChanged()
    {
        // arrange
        var defaultSettings = new BasicTestSettings { ValueType = 5 };
        var currentSettings = new BasicTestSettings();

        var settingsManager = new SettingsManager();
        settingsManager.Add(defaultSettings, currentSettings);

        var isInvoked = false;
        currentSettings.PropertyChanged += (_, _) => isInvoked = true;

        // act
        settingsManager.Reset<BasicTestSettings>();


        // assert
        Assert.True(isInvoked);
    }
}