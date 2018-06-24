using Repeats.Pages;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats
{
    public sealed partial class AskNameDialog : ContentDialog
    {
        public static string name;

        public AskNameDialog()
        {
            this.InitializeComponent();

            this.InitializeComponent();
            IsSecondaryButtonEnabled = false;
            ERROR.Visibility = Visibility.Collapsed;

            Load();
        }

        private void Load()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var ask1 = loader.GetString("ASK1");
            var ask2 = loader.GetString("ASK2");
            var ask3 = loader.GetString("ASK3");

            content.Title = ask1;
            content.PrimaryButtonText = ask2;
            content.SecondaryButtonText = ask3;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            content.Hide();
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

        private static async void ExceptionLongUps()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var strerr1 = loader.GetString("Error1");
            var strerr2 = loader.GetString("Error21");
            var strerr3 = loader.GetString("Error3");

            ContentDialog ExclUPS = new ContentDialog
            {
                Title = strerr1,
                Content = strerr2,
                CloseButtonText = strerr3
            };

            ContentDialogResult result = await ExclUPS.ShowAsync();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                name = ProjectName.Text;

                Frame frame = MainPage.FRAME;

                frame.Navigate(typeof(AddRepeats));

            }
            catch (Exception)
            {
                ExceptionUps();
            }
        }
        private async void changed_text(object sender, RoutedEventArgs e)
        {
            try
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var ask4 = loader.GetString("ASK4");
                var ask5 = loader.GetString("ASK5");
                var ask6 = loader.GetString("ASK6");

                StorageFolder MainStorage = ApplicationData.Current.LocalFolder;
                StorageFolder Fol = await MainStorage.GetFolderAsync("FOLDERS");

                name = ProjectName.Text;

                if (name.Contains("/") || name.Contains("\"") || name.Contains("\\") || name.Contains(":") || name.Contains("*") || name.Contains("?") || name.Contains("<") || name.Contains(">") || name.Contains("|"))
                {
                    string error = ask4 + Environment.NewLine + ask5 + "/ " + "\\ " + "\" " + ": " + "* " + "? " + "< " + "> " + "|";

                    ERROR.Text = error;
                    ERROR.Visibility = Visibility.Visible;

                    IsSecondaryButtonEnabled = false;
                }
                else
                {
                    IsSecondaryButtonEnabled = true;
                    ERROR.Visibility = Visibility.Collapsed;

                    if (ProjectName.Text == "")
                    {
                        IsSecondaryButtonEnabled = false;
                    }
                    else
                    {
                        if (await Fol.TryGetItemAsync(name) != null)
                        {
                            IsSecondaryButtonEnabled = false;

                            ERROR.Text = ask6;
                            ERROR.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            catch (Exception)
            {
                content.Hide();
            }

        }
    }
}
