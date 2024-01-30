using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.GridItems;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class WorkViewModel(IDatasource datasource)
: ItemViewModel<Work, WorkGridItem>(datasource, null)
{
    protected override int? DefaultNewItemChapter => null;
    protected override WorkGridItem Convert(int index, Event e, Work i, IEnumerable<Event> eventList)
    {
        return new WorkGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Type,
            eventList.Sum(o => o.Amount),
            e.DateEnd.Value
        );
    }
}
