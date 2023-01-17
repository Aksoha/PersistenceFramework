using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Holize.PersistenceFramework.Tests.Models;

public class BasicTestSettings : Settings
{
    private ObservableCollection<string> _collection = new();
    private Season _enumType;

    private Person _referenceType = new();
    private int _valueType;

    public BasicTestSettings()
    {
        Collection.CollectionChanged += Changed;
        ReferenceType.PropertyChanged += Changed;
    }

    public int ValueType
    {
        get => _valueType;
        set => SetField(ref _valueType, value);
    }

    public Person ReferenceType
    {
        get => _referenceType;
        set
        {
            var oldReference = _referenceType;
            var changed = SetField(ref _referenceType, value);
            if (!changed) return;
            oldReference.PropertyChanged -= Changed;
            ReferenceType.PropertyChanged += Changed;
        }
    }

    public ObservableCollection<string> Collection
    {
        get => _collection;
        set
        {
            var oldReference = _collection;
            var changed = SetField(ref _collection, value);
            if (!changed) return;
            oldReference.CollectionChanged -= Changed;
            Collection.CollectionChanged += Changed;
        }
    }

    public Season EnumType
    {
        get => _enumType;
        set => SetField(ref _enumType, value);
    }

    private void Changed(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    private void Changed(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged();
    }
}

public class Person : INotifyPropertyChanged
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;

    public string FirstName
    {
        get => _firstName;
        set => SetField(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetField(ref _lastName, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }
}