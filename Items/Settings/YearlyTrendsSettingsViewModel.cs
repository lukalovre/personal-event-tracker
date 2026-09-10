using System.Reactive;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace EventTracker.ViewModels;

public class YearlyTrendsSettingsViewModel : ViewModelBase
{
    private readonly YearlyTrendsSettings _settings;

    public YearlyTrendsSettingsViewModel()
    {
        _settings = Settings.Instance.YearlyTrends;
        Categories = _settings.Categories;
        MoveUp = ReactiveCommand.Create<YearlyTrendsCategorySetting>(MoveUpAction);
        MoveDown = ReactiveCommand.Create<YearlyTrendsCategorySetting>(MoveDownAction);
        Save = ReactiveCommand.Create(SaveAction);
    }

    public ObservableCollection<YearlyTrendsCategorySetting> Categories { get; }
    public ReactiveCommand<YearlyTrendsCategorySetting, Unit> MoveUp { get; }
    public ReactiveCommand<YearlyTrendsCategorySetting, Unit> MoveDown { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }

    private void MoveUpAction(YearlyTrendsCategorySetting category)
    {
        var index = Categories.IndexOf(category);
        if (index > 0)
        {
            Categories.Move(index, index - 1);
        }
    }

    private void MoveDownAction(YearlyTrendsCategorySetting category)
    {
        var index = Categories.IndexOf(category);
        if (index >= 0 && index < Categories.Count - 1)
        {
            Categories.Move(index, index + 1);
        }
    }

    private void SaveAction()
    {
        _settings.Normalize();
        Settings.Save();
    }
}
