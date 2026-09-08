using System.Threading.Tasks;
using Avalonia.Controls;

namespace EventTracker.ViewModels;

public interface IImagePickerViewModel
{
    Task PickImageAsync(Window window, bool isNewImage = false);
}