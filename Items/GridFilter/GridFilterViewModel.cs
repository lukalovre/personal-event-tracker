using System;
using System.Reactive;
using ReactiveUI;

namespace EventTracker.ViewModels;

public class GridFilterViewModel : ViewModelBase
{
    public GridFilterViewModel(IDataGrid dataGrid)
    {
        _dataGrid = dataGrid;
        Search = ReactiveCommand.Create(SearchAction);
        Reload = ReactiveCommand.Create(ReloadAction);
    }

    private IDataGrid _dataGrid;

    public ReactiveCommand<Unit, Unit> Search { get; }
    public ReactiveCommand<Unit, Unit> Reload { get; }
    private int _gridCountItems;

    public bool ShowYearFilter { get; set; } = true;

    public int GridCountItems
    {
        get => _gridCountItems;
        set => this.RaiseAndSetIfChanged(ref _gridCountItems, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    private int _yearFilter = DateTime.Now.Year;
    private string _searchText = string.Empty;

    public int YearFilter
    {
        get => _yearFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref _yearFilter, value);
            YearFilterChanged();
        }
    }

    private void YearFilterChanged()
    {
        GridCountItems = _dataGrid.ReloadData();
    }

    private void SearchAction()
    {
        SearchText = SearchText?.Trim() ?? string.Empty;
        GridCountItems = _dataGrid.ReloadData();
    }

    private void ReloadAction()
    {
        SearchText = string.Empty;
        GridCountItems = _dataGrid.ReloadData();
    }
}