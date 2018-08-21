using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RepeatsList : Page
    {
        public static string name;
        public static string OfficialName;
        public static bool IsEdit;
        public static string AV;
        public static StorageFolder folder;
        public static StorageFolder folder2;

        public RepeatsList()
        {
            this.InitializeComponent();

            GetFolder();

            this.ViewModel = new MainPageDataModel();
            IsEdit = false;
        }

        public MainPageDataModel ViewModel { get; set; }

        async void GetFolder()
        {
            folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Images");
        }

        private void TakeTestButton(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            name = button.Tag.ToString();
            RelativePanel rel = button.FindAscendantByName("RelativeRepeatsList") as RelativePanel;
            OfficialName = rel.Tag.ToString();

            Frame.Navigate(typeof(TakeTestPage), null, new DrillInNavigationTransitionInfo());    
        }

        public void ItemClick_Click(object sender, ItemClickEventArgs e)
        {
            var data = (RepeatsListData)e.ClickedItem;
            var item = e.ClickedItem;
            var g = GridRepeats.PrepareConnectedAnimation("image", item, "Person");
            OfficialName = data.ProjectName;
            name = data.TableName;
            AV = data.avatarTag;
            IsEdit = true;
            Frame.Navigate(typeof(AddEditRepeats), null, new SuppressNavigationTransitionInfo());
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
            Button del = sender as Button;
            string gettag = del.Tag.ToString();
            string imgtag = gettag;
            string IMGtag = imgtag = imgtag.Replace("R", "I");

            ContentDialog delete = new ContentDialog
            {
                Title = "Czy na pewno chcesz usunąć ten zestaw?",
                Content = "Usuwanie zestawów jest nieodwracalne.",
                PrimaryButtonText = "Usuń",
                CloseButtonText = "Anuluj"
            };

            ContentDialogResult result = await delete.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.Datas.Remove(new RepeatsListData() { TableName = gettag });

                using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                {
                    db.Open();
                    String tableCommand = "DROP TABLE " + gettag;
                    SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                    try
                    {
                        createTable.ExecuteReader();
                    }
                    catch (SqliteException)
                    {

                    }
                }

                using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                {
                    db.Open();
                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    insertCommand.CommandText = "DELETE FROM TitleTable WHERE TableName=" + "\"" + gettag + "\"";
                    try
                    {
                        insertCommand.ExecuteReader();
                    }
                    catch (SqliteException)
                    {
                        //Handle error
                        return;
                    }

                    db.Close();
                }

                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Images");
                var allimages = await folder.GetFilesAsync();
                var results = allimages.Where(x => x.Name.Contains(IMGtag));
                int count = results.Count();

                for(int i = 0; i < count; i++)
                {
                    var item = await folder.GetFileAsync(results.ElementAt(i).Name);
                    await item.DeleteAsync();
                }
            }
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddEditRepeats), null, new DrillInNavigationTransitionInfo());
        }
    }
}
