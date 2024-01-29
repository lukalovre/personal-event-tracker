using System;
using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.ViewModels.GridItems;

public record WorkGridItem(
    int ID,
    int Index,
    string Title,
    string Type,
    int Minutes,
    DateTime LastDate
) : IGridItem;
