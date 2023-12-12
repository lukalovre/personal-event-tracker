using System;

namespace AvaloniaApplication1.ViewModels.GridItems;

public record MyWorkGridItem(int ID, string Title, string Type, int minutes, DateTime LastDate);
