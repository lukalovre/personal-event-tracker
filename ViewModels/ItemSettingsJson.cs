namespace EventTracker.ViewModels;

public record ItemSettingsJson
{
    public int? NewItemAmountOverride { get; set; } = null;
    public int? DefaultNewItemChapter { get; set; } = 1;
    public bool DefaultNewItemCompleted { get; set; }
    public string DateTimeFilter { get; set; } = string.Empty;
    public int DefaultNewItemRating { get; set; } = 1;
    public int DefaultAddAmount { get; set; }
    public string DefaultNewItemPlatform { get; set; } = string.Empty;
    public bool OpenItemLinkUrl { get; set; }
    public string AmountVerb { get; set; } = "minutes";
    public bool DefaultNewItemBookmakred { get; set; }
    public float AmountToMinutesModifier { get; set; } = 1f;
    public bool IsFullAmountDefaultValue { get; set; } = true;
    public eAmountType? AmountType { get; set; } = eAmountType.Minutes;
    public bool ShowYearFilter { get; set; } = true;
}