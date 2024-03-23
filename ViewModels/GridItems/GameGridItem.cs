using System;
using AvaloniaApplication1.Models;

public record GameGridItem(int ID, string Title, int Year, string Platform, int Time, bool Completed, DateTime? LastDate) : IGridItem;
