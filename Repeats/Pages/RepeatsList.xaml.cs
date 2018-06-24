using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Data.Sqlite;

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

        public RepeatsList()
        {
            this.InitializeComponent();

            this.ViewModel = new MainPageDataModel();
        }

        public MainPageDataModel ViewModel { get; set; }

        private void TakeTestButton(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            name = button.Tag.ToString();
            Frame.Navigate(typeof(TakeTestPage));
        }

        public void ItemClick_Click(object sender, ItemClickEventArgs e)
        {
            var data = (RepeatsListData)e.ClickedItem;
            OfficialName = data.ProjectName;
            name = data.TableName;
            Frame.Navigate(typeof(EditItems));
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button del = sender as Button;
            string gettag = del.Tag.ToString();

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

            ViewModel.Datas.Remove(new RepeatsListData() { TableName = gettag });
        }

        private async void AddClick(object sender, RoutedEventArgs e)
        {
            AskNameDialog dialog = new AskNameDialog();
            await dialog.ShowAsync();
        }

        private void BellClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TestResults));
        }
    }
}
