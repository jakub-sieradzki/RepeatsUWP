using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InkPage : Page
    {
        public InkPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            inkCanvas.InkPresenter.InputDeviceTypes =
            Windows.UI.Core.CoreInputDeviceTypes.Mouse |
            Windows.UI.Core.CoreInputDeviceTypes.Touch |
            Windows.UI.Core.CoreInputDeviceTypes.Pen;
        }

        private static async void ExceptionUps()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var strerr1 = loader.GetString("Error1");
            var strerr2 = loader.GetString("Error2");
            var strerr3 = loader.GetString("Error3");

            ContentDialog ExcUPS = new ContentDialog
            {
                Title = strerr1,
                Content = strerr2,
                CloseButtonText = strerr3
            };

            ContentDialogResult result = await ExcUPS.ShowAsync();
        }

        private static async void ExceptionLoadUps()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var strerr1 = loader.GetString("Error1");
            var strerr2 = loader.GetString("Error4");
            var strerr3 = loader.GetString("Error3");
            ContentDialog ExclUPS = new ContentDialog
            {
                Title = strerr1,
                Content = strerr2,
                CloseButtonText = strerr3
            };

            ContentDialogResult result = await ExclUPS.ShowAsync();
        }

        private async void buttonSave_ClickAsync(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var ink1 = loader.GetString("InkProject");
            try
            {
                IReadOnlyList<InkStroke> currentStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

                if (currentStrokes.Count > 0)
                {
                    Windows.Storage.Pickers.FileSavePicker savePicker =
                        new Windows.Storage.Pickers.FileSavePicker();
                    savePicker.SuggestedStartLocation =
                        Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                    savePicker.FileTypeChoices.Add(
                        "GIF with embedded ISF",
                        new List<string>() { ".gif" });

                    savePicker.DefaultFileExtension = ".gif";
                    savePicker.SuggestedFileName = ink1;

                    Windows.Storage.StorageFile file =
                        await savePicker.PickSaveFileAsync();

                    if (file != null)
                    {
                        Windows.Storage.CachedFileManager.DeferUpdates(file);

                        IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                        using (IOutputStream outputStream = stream.GetOutputStreamAt(0))
                        {
                            await inkCanvas.InkPresenter.StrokeContainer.SaveAsync(outputStream);
                            await outputStream.FlushAsync();
                        }
                        stream.Dispose();

                        Windows.Storage.Provider.FileUpdateStatus status =
                            await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);

                        if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                        {

                        }
                        else
                        {

                        }
                    }

                    else
                    {

                    }
                }
            }
            catch (Exception)
            {
                ExceptionUps();
            }
        }

        private async void buttonLoad_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.Storage.Pickers.FileOpenPicker openPicker =
                    new Windows.Storage.Pickers.FileOpenPicker();
                openPicker.SuggestedStartLocation =
                    Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".gif");

                Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                    using (var inputStream = stream.GetInputStreamAt(0))
                    {
                        await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(inputStream);
                    }
                    stream.Dispose();
                }
                else
                {

                }
            }
            catch (Exception)
            {
                ExceptionLoadUps();
            }

        }
    }
}
