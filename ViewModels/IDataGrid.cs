namespace EventTracker.ViewModels;

public interface IDataGrid : ILoadOnSelection
{
    public int ReloadData();
    public void SelectFirstItem();
}

public interface ILoadOnSelection
{
    public void LoadOnSelection();
}