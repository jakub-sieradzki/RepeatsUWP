using Microsoft.Data.Sqlite;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public class bind2
    {
        public int ClickCount { get; set; }
        public string GetQuestion { get; set; }
        public string GetAnswer { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var bi = (bind2)obj;
            return this.ClickCount == bi.ClickCount;
        }
        public override int GetHashCode()
        {
            return ClickCount ^ 7;
        }
    }

    public class Bind2ViewModel
    {
        private ObservableCollection<bind2> editRepeat = new ObservableCollection<bind2>();
        public ObservableCollection<bind2> EditRepeat { get { return this.editRepeat; } }
        public Bind2ViewModel()
        {

        }
    }

    public sealed partial class EditItems : Page
    {
        public int all;

        public EditItems()
        {
            this.InitializeComponent();

            Ring.IsActive = false;

            all = 0;

            this.ViewModel2 = new Bind2ViewModel();

            AskNameEdit.Text = RepeatsList.OfficialName;

            Load();

            ConnectedAnimation imageAnimation =
                ConnectedAnimationService.GetForCurrentView().GetAnimation("image");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(PicEdit);
            }
        }

        public Bind2ViewModel ViewModel2 { get; set; }

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

        private void Load()
        {
            string NAME = RepeatsList.name;

            List<string> GetQuestions = GetFromDB.GrabData(NAME, "question");
            List<string> GetAnswers = GetFromDB.GrabData(NAME, "answer");

            int count = GetQuestions.Count;
            count--;

            for(int i = 0; i <= count; i++)
            {
                ViewModel2.EditRepeat.Add(new bind2() { ClickCount = i, GetQuestion = GetQuestions[i], GetAnswer = GetAnswers[i] });
                all++;
            }
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

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string strtag = button.Tag.ToString();

            var finddd = button.FindAscendantByName("REL");
            //finddd.

            int count = Int32.Parse(strtag);

            Grid find = button.Parent as Grid;

            var find2 = find.FindName("REL") as RelativePanel;

            var q = find2.FindName("quest") as TextBox;
            var a = find2.FindName("answer") as TextBox;
            q.Text = "";
            a.Text = "";

            ViewModel2.EditRepeat.Remove(new bind2() { ClickCount = count });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var items = GRID.Items;
            var findrelative = GRID.FindDescendants<RelativePanel>();
            var listrel = findrelative.ToList();
            int relcount = listrel.Count;
            relcount--;

            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            date = "R" + date;

            string realDate = DateTime.Now.ToShortDateString();

            string NAME = RepeatsList.name;
            string official = RepeatsList.OfficialName;

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
                catch (SqliteException)
                {

                }
                #endregion

                #region Get & save sets
                for (int i = 1; i <= relcount; i++)
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

                #region Insert values into TitleTable
                SqliteCommand insertCommand2 = new SqliteCommand();
                insertCommand2.Connection = db;

                insertCommand2.CommandText = "INSERT INTO TitleTable VALUES (NULL, @title, @TableName, @CreateDate, @IsEnabled);";
                insertCommand2.Parameters.AddWithValue("@title", official);
                insertCommand2.Parameters.AddWithValue("@TableName", date);
                insertCommand2.Parameters.AddWithValue("@CreateDate", realDate);
                insertCommand2.Parameters.AddWithValue("@IsEnabled", "true");
                try
                {
                    insertCommand2.ExecuteReader();
                }
                catch (SqliteException)
                {
                    return;
                }
                #endregion

                #region Drop table
                String tableCommand2 = "DROP TABLE " + NAME;
                SqliteCommand createTable2 = new SqliteCommand(tableCommand2, db);
                try
                {
                    createTable2.ExecuteReader();
                }
                catch (SqliteException)
                {

                }
                #endregion

                #region Delete from RepeatsList
                SqliteCommand insertCommand3 = new SqliteCommand();
                insertCommand3.Connection = db;

                insertCommand3.CommandText = "DELETE FROM TitleTable WHERE TableName=" + "\"" + NAME + "\"";
                try
                {
                    insertCommand3.ExecuteReader();
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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            all++;
            ViewModel2.EditRepeat.Add(new bind2() { ClickCount = all });
        }
    }
}
