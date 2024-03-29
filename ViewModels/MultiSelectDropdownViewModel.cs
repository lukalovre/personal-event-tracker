using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AvaloniaApplication1.ViewModels;

public class MultiSelectDropdownViewModel : INotifyPropertyChanged
{
    public class PersonCheckbox
    {
        public bool IsSelected { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ID { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    private string _searchText = string.Empty;
    private ObservableCollection<PersonCheckbox> _items = [];
    private ObservableCollection<PersonCheckbox> _filteredItems = [];

    public MultiSelectDropdownViewModel()
    {
        Items = new ObservableCollection<PersonCheckbox>(PeopleManager.Instance.GetComboboxList()
        .Select(o => new PersonCheckbox
        {
            ID = o.ID,
            Name = PeopleManager.Instance.GetDisplayName(o.ID),
            FullName = $"{o.FirstName} {o.LastName} {o.Nickname}"
        }));

        FilteredItems = new ObservableCollection<PersonCheckbox>(Items.OrderBy(o => o.IsSelected));
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterItems();
        }
    }

    public ObservableCollection<PersonCheckbox> Items
    {
        get => _items;
        set
        {
            _items = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<PersonCheckbox> FilteredItems
    {
        get => _filteredItems;
        set
        {
            _filteredItems = value;
            OnPropertyChanged();
        }
    }

    private void FilterItems()
    {
        FilteredItems.Clear();

        foreach (var item in Items)
        {
            if (string.IsNullOrWhiteSpace(SearchText) || item.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            {
                FilteredItems.Add(item);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}