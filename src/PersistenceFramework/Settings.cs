using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Holize.PersistenceFramework;

/// <summary>
///     A base class representing application settings.
/// </summary>
public abstract class Settings : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///     Invokes <see cref="PropertyChanged" />.
    /// </summary>
    /// <param name="propertyName">The name of calling property.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    /// <summary>
    ///     Changes the value of the <paramref name="field" /> when <paramref name="value" /> is not the same as the
    ///     <paramref name="field" />.
    ///     Invokes <see cref="PropertyChanged" /> if change was performed.
    /// </summary>
    /// <param name="field">A field to change.</param>
    /// <param name="value">The new value of the <paramref name="field" />.</param>
    /// <param name="propertyName">The name of calling property.</param>
    /// <typeparam name="TValue">The type of field to set.</typeparam>
    /// <returns><see langword="true" /> when <paramref name="field" /> was changed, otherwise <see langword="false" />.</returns>
    protected bool SetField<TValue>(ref TValue field, TValue value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<TValue>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}