using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace EventTracker.Views;

public partial class StarRatingView : UserControl
{
    public static readonly StyledProperty<int?> RatingProperty =
        AvaloniaProperty.Register<StarRatingView, int?>(nameof(Rating));

    public int? Rating
    {
        get => GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
    }

    public StarRatingView()
    {
        InitializeComponent();
        UpdateStars();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == RatingProperty)
        {
            UpdateStars();
        }
    }

    private void OnStarClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton { Tag: string value }
            || !int.TryParse(value, out var rating))
        {
            return;
        }

        Rating = rating;
        UpdateStars();
    }

    private void UpdateStars()
    {
        var rating = Rating.GetValueOrDefault();
        StarOne.IsChecked = rating >= 1;
        StarTwo.IsChecked = rating >= 2;
        StarThree.IsChecked = rating >= 3;
        StarFour.IsChecked = rating >= 4;
        StarFive.IsChecked = rating >= 5;
    }
}