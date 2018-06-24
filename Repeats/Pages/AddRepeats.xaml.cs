using System;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public class bind1
    {
        public int ClickCount { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var bi = (bind1)obj;
            return this.ClickCount == bi.ClickCount;
        }
        public override int GetHashCode()
        {
            return ClickCount ^ 7;
        }
    }

    public class BindViewModel
    {
        private ObservableCollection<bind1> addRepeat = new ObservableCollection<bind1>();
        public ObservableCollection<bind1> AddRepeat { get { return this.addRepeat; } }
        public BindViewModel()
        {

        }
    }

    public sealed partial class AddRepeats : Page
    {
        public StorageFolder MainStorage;
        public int count;

        public AddRepeats()
        {
            this.InitializeComponent();

            Ring.IsActive = false;

            STORAGE();

            count = 0;

            this.ViewModel1 = new BindViewModel();

            ViewModel1.AddRepeat.Add(new bind1() { ClickCount = count });
        }

        public BindViewModel ViewModel1 { get; set; }

        private void STORAGE()
        {
            MainStorage = ApplicationData.Current.LocalFolder;
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string strtag = button.Tag.ToString();

            int count = Int32.Parse(strtag);

            Grid find = button.Parent as Grid;

            var find2 = find.FindName("REL") as RelativePanel;

            var q = find2.FindName("quest") as TextBox;
            var a = find2.FindName("answer") as TextBox;
            q.Text = "";
            a.Text = "";

            ViewModel1.AddRepeat.Remove(new bind1() { ClickCount = count });
        }

        public void NewItemClick(object sender, RoutedEventArgs e)
        {
            count++;
            ViewModel1.AddRepeat.Add(new bind1() { ClickCount = count });

            string date = DateTime.Now.ToLongDateString();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Ring.Visibility = Visibility.Visible;
            Ring.IsActive = true;

            var findrelative = GRID.FindDescendants<RelativePanel>();
            var listrel = findrelative.ToList();
            int relcount = listrel.Count;

            relcount--;

            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            date = "R" + date;

            string realDate = DateTime.Now.ToShortDateString();
            string getname = AskNameDialog.name;

            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();

                #region Create table
                String tableCommand = "CREATE TABLE IF NOT EXISTS " + date + " (id INTEGER PRIMARY KEY AUTOINCREMENT, question NVARCHAR(2048) NULL, answer NVARCHAR(2048) NULL)";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                try
                {
                    createTable.ExecuteReader();
                }
                catch(SqliteException)
                {

                }
                #endregion

                #region Get & save sets
                for (int i = 0; i <= relcount; i++)
                {
                    RelativePanel panel = listrel[i];
                    TextBox questbox = panel.FindChildByName("quest") as TextBox;
                    TextBox answerbox = panel.FindChildByName("answer") as TextBox;

                    string question = questbox.Text;
                    string answer = answerbox.Text;

                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    insertCommand.CommandText = "INSERT INTO " + date + " VALUES (NULL, @question, @answer);";
                    insertCommand.Parameters.AddWithValue("@question", question);
                    insertCommand.Parameters.AddWithValue("@answer", answer);

                    try
                    {
                        insertCommand.ExecuteReader();
                    }
                    catch (SqliteException)
                    {
                        return;
                    }
                }
                #endregion

                #region save to RepeatsList
                SqliteCommand insertCommand2 = new SqliteCommand();
                insertCommand2.Connection = db;

                insertCommand2.CommandText = "INSERT INTO TitleTable VALUES (NULL, @title, @TableName, @CreateDate);";
                insertCommand2.Parameters.AddWithValue("@title", getname);
                insertCommand2.Parameters.AddWithValue("@TableName", date);
                insertCommand2.Parameters.AddWithValue("@CreateDate", realDate);
                try
                {
                    insertCommand2.ExecuteReader();
                }
                catch (SqliteException)
                {
                    return;
                }
                #endregion

                db.Close();
            }

            Frame.Navigate(typeof(RepeatsList));
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

        private static async void CountDialog()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var stratt1 = loader.GetString("Attention1");
            var stratt2 = loader.GetString("Attention2");

            ContentDialog ConDia = new ContentDialog
            {
                Title = stratt1,
                Content = stratt2,
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await ConDia.ShowAsync();
        }

        private static async void WriteDialog()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var stratt11 = loader.GetString("Attention1");
            var stratt22 = loader.GetString("Attention3");

            ContentDialog WriDia = new ContentDialog
            {
                Title = stratt11,
                Content = stratt22,
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await WriDia.ShowAsync();
        }

        private async void ASKTIME()
        {
            try
            {
                var a = await MainStorage.TryGetItemAsync("time.txt");

                if (a == null)
                {
                    AskTimeDialog TIME = new AskTimeDialog();
                    await TIME.ShowAsync();

                    Ring.IsActive = false;
                    Add.IsEnabled = true;
                    Save.IsEnabled = true;
                }
                else
                {
                    var taskName = "RepeatsNotificationTask";

                    foreach (var cur in BackgroundTaskRegistration.AllTasks)
                    {
                        if (cur.Value.Name == taskName)
                        {

                        }
                        else
                        {
                            RegisterTask.RegisterBackgroundTask();
                        }
                    }

                    Frame.Navigate(typeof(RepeatsList));
                }
            }
            catch (Exception)
            {
                ExceptionUps();
            }

        }
    }
}
