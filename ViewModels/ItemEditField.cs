using System;
using System.Globalization;
using System.Reflection;
using System.Reactive;
using System.Text.RegularExpressions;
using ReactiveUI;

namespace EventTracker.ViewModels;

public abstract class ItemEditField(object item, PropertyInfo property) : ReactiveObject
{
    protected object Item { get; } = item;
    protected PropertyInfo Property { get; } = property;
    public string Label { get; } = Regex.Replace(property.Name, "([a-z0-9])([A-Z])", "$1 $2");

    public virtual bool TryApply() => true;

    public static ItemEditField Create(object item, PropertyInfo property)
    {
        var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (type == typeof(bool))
        {
            return new ItemEditBooleanField(item, property);
        }

        if (type == typeof(DateTime))
        {
            return new ItemEditDateTimeField(item, property);
        }

        if (type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long)
            || type == typeof(float) || type == typeof(double) || type == typeof(decimal))
        {
            return new ItemEditNumberField(item, property);
        }

        return new ItemEditTextField(item, property);
    }
}

public sealed class ItemEditTextField(object item, PropertyInfo property) : ItemEditField(item, property)
{
    private string _value = property.GetValue(item)?.ToString() ?? string.Empty;

    public string Value
    {
        get => _value;
        set
        {
            this.RaiseAndSetIfChanged(ref _value, value);
            if (Property.PropertyType == typeof(string))
            {
                Property.SetValue(Item, value);
            }
        }
    }

    public override bool TryApply()
    {
        if (Property.PropertyType == typeof(string))
        {
            Property.SetValue(Item, Value);
            return true;
        }

        var targetType = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;
        if (string.IsNullOrWhiteSpace(Value) && Nullable.GetUnderlyingType(Property.PropertyType) is not null)
        {
            Property.SetValue(Item, null);
            return true;
        }

        try
        {
            Property.SetValue(Item, Convert.ChangeType(Value, targetType, CultureInfo.CurrentCulture));
            return true;
        }
        catch (Exception exception) when (exception is FormatException or InvalidCastException or OverflowException)
        {
            return false;
        }
    }
}

public sealed class ItemEditNumberField(object item, PropertyInfo property) : ItemEditField(item, property)
{
    private decimal? _value = property.GetValue(item) is { } value
        ? Convert.ToDecimal(value, CultureInfo.CurrentCulture)
        : null;

    public decimal? Value
    {
        get => _value;
        set
        {
            this.RaiseAndSetIfChanged(ref _value, value);
            var targetType = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;
            if (value.HasValue)
            {
                Property.SetValue(Item, Convert.ChangeType(value.Value, targetType, CultureInfo.CurrentCulture));
            }
            else if (Nullable.GetUnderlyingType(Property.PropertyType) is not null)
            {
                Property.SetValue(Item, null);
            }
        }
    }
}

public sealed class ItemEditBooleanField(object item, PropertyInfo property) : ItemEditField(item, property)
{
    private bool _value = (bool)(property.GetValue(item) ?? false);

    public bool Value
    {
        get => _value;
        set
        {
            this.RaiseAndSetIfChanged(ref _value, value);
            Property.SetValue(Item, value);
        }
    }
}

public sealed class ItemEditDateTimeField : ItemEditField
{
    private bool _isPickerOpen;
    private DateTimeOffset? _pendingDate;
    private TimeSpan? _pendingTime;

    public ItemEditDateTimeField(object item, PropertyInfo property) : base(item, property)
    {
        TogglePicker = ReactiveCommand.Create(TogglePickerAction);
        ConfirmDateTime = ReactiveCommand.Create(ConfirmDateTimeAction);
    }

    public ReactiveCommand<Unit, Unit> TogglePicker { get; }
    public ReactiveCommand<Unit, Unit> ConfirmDateTime { get; }

    public string DisplayValue => ((DateTime?)Property.GetValue(Item))?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;

    public bool IsPickerOpen
    {
        get => _isPickerOpen;
        set
        {
            this.RaiseAndSetIfChanged(ref _isPickerOpen, value);
            if (value)
            {
                var current = (DateTime?)Property.GetValue(Item) ?? DateTime.Now;
                PendingDate = new DateTimeOffset(current);
                PendingTime = current.TimeOfDay;
            }
        }
    }

    public DateTimeOffset? PendingDate
    {
        get => _pendingDate;
        set => this.RaiseAndSetIfChanged(ref _pendingDate, value);
    }

    public TimeSpan? PendingTime
    {
        get => _pendingTime;
        set => this.RaiseAndSetIfChanged(ref _pendingTime, value);
    }

    private void TogglePickerAction()
    {
        IsPickerOpen = !IsPickerOpen;
    }

    private void ConfirmDateTimeAction()
    {
        if (PendingDate is null || PendingTime is null)
        {
            return;
        }

        Property.SetValue(Item, PendingDate.Value.Date.Add(PendingTime.Value));
        this.RaisePropertyChanged(nameof(DisplayValue));
        IsPickerOpen = false;
    }
}