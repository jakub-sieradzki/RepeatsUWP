using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchListPage : Page
    {
        public IReadOnlyList<StorageFolder> abcd;
        public static string name;

        public SearchListPage()
        {
            this.InitializeComponent();

            Load();
        }

        private async void Load()
        {
            Start.Visibility = Visibility.Collapsed;

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFolder c = await storageFolder.GetFolderAsync("FOLDERS");
            abcd = await c.GetFoldersAsync();

            ListViewI.ItemsSource = abcd;
        }

        private void ItemClick_Click(object sender, ItemClickEventArgs e)
        {
            Start.Visibility = Visibility.Visible;
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

        private void Next(object sender, RoutedEventArgs e)
        {
            try
            {
                int cc = ListViewI.SelectedIndex;

                name = abcd[cc].DisplayName.ToString();

                Frame.Navigate(typeof(SearchPage));
            }
            catch (Exception)
            {
                ExceptionUps();
            }

        }
    }
}
