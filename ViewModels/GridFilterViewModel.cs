using System;
using System.Reactive;
using ReactiveUI;

namespace AvaloniaApplication1.ViewModels;

public class GridFilterViewModel : ViewModelBase
{
    public GridFilterViewModel(IDataGrid dataGrid)
    {
        _dataGrid = dataGrid;
        Search = ReactiveCommand.Create(SearchAction);
    }

    private IDataGrid _dataGrid;

    public ReactiveCommand<Unit, Unit> Search { get; }
    private int _gridCountItems;

    public int GridCountItems
    {
        get => _gridCountItems;
        set => this.RaiseAndSetIfChanged(ref _gridCountItems, value);
    }

    public string SearchText { get; set; }

    private int _yearFilter = DateTime.Now.Year;

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
}