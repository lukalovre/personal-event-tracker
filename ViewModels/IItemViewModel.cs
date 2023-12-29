using System.Collections.ObjectModel;

namespace AvaloniaApplication1.ViewModels;

public interface IItemViewModel<T>
{
    EventViewModel EventViewModel { get; }
    bool UseNewDate { get; set; }

    static ObservableCollection<PersonComboBoxItem> PeopleList { get; }

    PersonComboBoxItem SelectedPerson { get; set; }

}