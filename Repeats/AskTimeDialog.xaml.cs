using Repeats.Pages;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats
{
    public sealed partial class AskTimeDialog : ContentDialog
    {
        public static int time;

        public AskTimeDialog()
        {
            this.InitializeComponent();

            second.IsChecked = true;

            Load();
        }

        private void Load()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var time1 = loader.GetString("TIME1");
            var time2 = loader.GetString("TIME2");
            var time3 = loader.GetString("TIME3");

            TimeContent.Title = time1;
            TimeContent.PrimaryButtonText = time2;
            TimeContent.SecondaryButtonText = time3;

        }

        private void TimeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string a = rb.Tag.ToString();
            switch (a)
            {
                case "15":
                    time = 15;
                    break;
                case "30":
                    time = 30;
                    break;
                case "45":
                    time = 45;
                    break;
                case "60":
                    time = 60;
                    break;
            }
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

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            TimeContent.Hide();
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                Frame frame = MainPage.FRAME;

                StorageFolder Storage = ApplicationData.Current.LocalFolder;
                StorageFile Storage1 = await Storage.CreateFileAsync("time.txt", CreationCollisionOption.ReplaceExisting);
                StorageFile Storage2 = await Storage.GetFileAsync("time.txt");
                await FileIO.WriteTextAsync(Storage2, time.ToString());

                RegisterTask.RegisterBackgroundTask();
                frame.Navigate(typeof(RepeatsList));
            }
            catch (Exception)
            {
                ExceptionUps();
            }

        }
    }
}
