using System;
using System.Collections.ObjectModel;

namespace AvaloniaApplication1.ViewModels;

public record ItemSettings
{
    public int? NewItemAmountOverride { get; set; } = null;
    public int? DefaultNewItemChapter { get; set; } = 1;
    public bool DefaultNewItemCompleted { get; set; }
    public DateTime? DateTimeFilter { get; set; }
    public int DefaultNewItemRating { get; set; } = 1;
    public int DefaultAddAmount { get; set; }
    public string DefaultNewItemPlatform { get; set; } = string.Empty;
    public bool OpenItemLinkUrl { get; set; }
    public string AmountVerb { get; set; } = "minutes";
    public ObservableCollection<string> PlatformTypes { get; set; } = [];
    public bool DefaultNewItemBookmakred { get; set; }
    public float AmountToMinutesModifier { get; set; } = 1f;
    public bool IsFullAmountDefaultValue { get; set; } = true;
}