using System;
using AvaloniaApplication1.Models;

public record GameGridItem(
    int ID,
    int Index,
    string Title,
    int Year,
    string Platform,
    int Time,
    bool Completed,
    DateTime? LastDate,
    int DaysAgo
) : IGridItem;
