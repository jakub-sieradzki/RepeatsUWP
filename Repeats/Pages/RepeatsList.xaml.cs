using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RepeatsList : Page
    {
        public static string clicked;
        public static int items;
        public static string name;
        public StorageFolder c;
        public IReadOnlyList<StorageFolder> abcd;

        public RepeatsList()
        {
            this.InitializeComponent();

            Load();

            Edit.Visibility = Visibility.Collapsed;
            Delete.Visibility = Visibility.Collapsed;
        }

        private async void Load()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            c = await storageFolder.GetFolderAsync("FOLDERS");
            abcd = await c.GetFoldersAsync();

            ListViewI.ItemsSource = abcd;
            items = ListViewI.Items.Count;
        }

        public void ItemClick_Click(object sender, ItemClickEventArgs e)
        {
            Edit.Visibility = Visibility.Visible;
            Delete.Visibility = Visibility.Visible;
            Add.Visibility = Visibility.Collapsed;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int cc = ListViewI.SelectedIndex;

                name = abcd[cc].DisplayName;

                Frame.Navigate(typeof(EditItems));
            }
            catch (Exception)
            {
                ExceptionUps();
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

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var delete1 = loader.GetString("DELETE1");
            var delete2 = loader.GetString("DELETE2");
            var delete3 = loader.GetString("DELETE3");

            try
            {
                int cc = ListViewI.SelectedIndex;

                name = abcd[cc].DisplayName;

                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFolder c = await storageFolder.GetFolderAsync("FOLDERS");
                StorageFolder d = await c.GetFolderAsync(name);

                MessageDialog dialog = new MessageDialog(delete1);
                dialog.Commands.Add(new UICommand(delete2) { Id = "delete" });
                dialog.Commands.Add(new UICommand(delete3));

                var result = await dialog.ShowAsync();
                if (object.Equals(result.Id, "delete"))
                {
                    await d.DeleteAsync();
                    abcd = await c.GetFoldersAsync();
                    ListViewI.ItemsSource = abcd;
                    items = ListViewI.Items.Count;
                }

                int count = ListViewI.Items.Count;

                if (count == 0)
                {
                    Settings.CancelTask();

                    Edit.Visibility = Visibility.Collapsed;
                    Delete.Visibility = Visibility.Collapsed;
                    Add.Visibility = Visibility.Visible;
                }
                else
                {
                    Edit.Visibility = Visibility.Collapsed;
                    Delete.Visibility = Visibility.Collapsed;
                    Add.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                ExceptionUps();
            }

        }

        private async void AddClick(object sender, RoutedEventArgs e)
        {
            AskNameDialog dialog = new AskNameDialog();
            await dialog.ShowAsync();
        }
    }
}
