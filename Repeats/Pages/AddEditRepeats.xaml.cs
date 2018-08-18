using Microsoft.Data.Sqlite;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public class AddEditBind
    {
        public int ClickCount { get; set; }
        public string GetQuestion { get; set; }
        public string GetAnswer { get; set; }
        public string GetImage { get; set; }
        public string ImageTag { get; set; }
        public Visibility visibility { get; set; }
        public bool enabled { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var bi = (AddEditBind)obj;
            return this.ClickCount == bi.ClickCount;
        }
        public override int GetHashCode()
        {
            return ClickCount ^ 7;
        }
    }

    public class AddEditBindModel
    {
        private ObservableCollection<AddEditBind> addEditBinds = new ObservableCollection<AddEditBind>();
        public ObservableCollection<AddEditBind> AddEditBinds { get { return this.addEditBinds; } }
        public AddEditBindModel()
        {

        }
    }

    public sealed partial class AddEditRepeats : Page
    {
        public int Count;
        public bool edit;
        public string tablename;
        public string date;
        public string realDate;

        public AddEditRepeats()
        {
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;

            this.InitializeComponent();

            this.EditBindModel = new AddEditBindModel();

            date = DateTime.Now.ToString("yyyyMMddHHmmss");
            date = "R" + date;

            realDate = DateTime.Now.ToShortDateString();

            Count = 0;

            edit = RepeatsList.IsEdit;

            if(edit)
            {
                ConnectedAnimation imageAnimation =
                    ConnectedAnimationService.GetForCurrentView().GetAnimation("image");
                if (imageAnimation != null)
                {
                    imageAnimation.TryStart(Pic);
                }

                tablename = RepeatsList.name;
                string name = RepeatsList.OfficialName;

                AskName.Text = name;

                List<string> questions = GetFromDB.GrabData(tablename, "question");
                List<string> answers = GetFromDB.GrabData(tablename, "answer");
                List<string> images = GetFromDB.GrabData(tablename, "image");

                int count = questions.Count;

                for(int i = 0; i < count; i++)
                {
                    string QUESTION = questions.ElementAt(i);
                    string ANSWER = answers.ElementAt(i);
                    string IMAGE = images.ElementAt(i);

                    if(IMAGE == "")
                    {
                        EditBindModel.AddEditBinds.Add(new AddEditBind() { ClickCount = Count, GetQuestion = QUESTION, GetAnswer = ANSWER, GetImage = IMAGE, visibility = Visibility.Collapsed, enabled = true, ImageTag = "" });
                    }
                    else
                    {
                        EditBindModel.AddEditBinds.Add(new AddEditBind() { ClickCount = Count, GetQuestion = QUESTION, GetAnswer = ANSWER, GetImage = RepeatsList.folder.Path + "\\" + IMAGE, visibility = Visibility.Visible, enabled = false, ImageTag = IMAGE});
                    }

                    Count++;
                }
            }
            else
            {
                EditBindModel.AddEditBinds.Add(new AddEditBind() { ClickCount = Count, visibility = Visibility.Collapsed, enabled = true, ImageTag = "" });
                Count++;
            }
        }

        public AddEditBindModel EditBindModel { get; set; }

        private void NewItemClick(object sender, RoutedEventArgs e)
        {
            EditBindModel.AddEditBinds.Add(new AddEditBind() { ClickCount = Count, enabled = true, visibility = Visibility.Collapsed, ImageTag = "" });
            Count++;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Ring.Visibility = Visibility.Visible;
            Ring.IsActive = true;

            var findrelative = GRID.FindDescendants<RelativePanel>();
            var listrel = findrelative.ToList();
            int relcount = GRID.Items.Count;

            string getname = AskName.Text;

            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();

                #region Create table
                String tableCommand = "CREATE TABLE IF NOT EXISTS " + date + " (id INTEGER PRIMARY KEY AUTOINCREMENT, question NVARCHAR(2048) NULL, answer NVARCHAR(2048) NULL, image NVARCHAR(2048) NULL)";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                //try
                //{
                createTable.ExecuteReader();
                //}
                //catch(SqliteException)
                //{

                //}
                #endregion

                #region Get & save sets
                for (int i = 1; i <= relcount; i++)
                {
                    RelativePanel panel = listrel[i];
                    TextBox questbox = panel.FindChildByName("quest") as TextBox;
                    TextBox answerbox = panel.FindChildByName("answer") as TextBox;
                    Image photoButton = panel.FindChildByName("ImagePreview") as Image;

                    string question = questbox.Text;
                    string answer = answerbox.Text;
                    string tag = photoButton.Tag.ToString();

                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    insertCommand.CommandText = "INSERT INTO " + date + " VALUES (NULL, @question, @answer, @image);";
                    insertCommand.Parameters.AddWithValue("@question", question);
                    insertCommand.Parameters.AddWithValue("@answer", answer);
                    insertCommand.Parameters.AddWithValue("@image", tag);
                    //try
                    //{
                    insertCommand.ExecuteReader();
                    //}
                    //catch (SqliteException)
                    //{
                    //    return;
                    //}
                }
                #endregion

                #region save to RepeatsList
                SqliteCommand insertCommand2 = new SqliteCommand();
                insertCommand2.Connection = db;

                insertCommand2.CommandText = "INSERT INTO TitleTable VALUES (NULL, @title, @TableName, @CreateDate, @IsEnabled);";
                insertCommand2.Parameters.AddWithValue("@title", getname);
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

                if(edit)
                {
                    #region Drop table
                    String tableCommand2 = "DROP TABLE " + tablename;
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

                    insertCommand3.CommandText = "DELETE FROM TitleTable WHERE TableName=" + "\"" + tablename + "\"";
                    try
                    {
                        insertCommand3.ExecuteReader();
                    }
                    catch (SqliteException)
                    {
                        return;
                    }
                    #endregion
                }

                db.Close();
            }

            notifi();


            //AskTimeDialog TIME = new AskTimeDialog();
            //await TIME.ShowAsync();

            Frame.Navigate(typeof(RepeatsList));
        }

        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            Button but = sender as Button;
            string tag = but.Tag.ToString();


            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("QImages");

            tag = tag.Replace(folder.Path.ToString() +"\\", "");

            var file = await folder.GetFileAsync(tag);
            await file.DeleteAsync();

            RelativePanel find = but.FindAscendantByName("REL") as RelativePanel;
            Image img = find.FindChildByName("ImagePreview") as Image;
            img.Visibility = Visibility.Collapsed;
            Button btn = find.FindChildByName("AddPhoto") as Button;

            btn.Tag = "";
            img.Tag = "";
            btn.IsEnabled = true;
            but.Visibility = Visibility.Collapsed;
        }

        private async void AddPhoto_Click(object sender, RoutedEventArgs e)
        {
            Button but = sender as Button;

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                string type = file.FileType;
                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("QImages");
                StorageFile File = await file.CopyAsync(folder);

                string Date = date;
                string DATE = Date.Replace("R", "I");

                await File.RenameAsync(DATE + type, NameCollisionOption.GenerateUniqueName);

                string realName = File.Name;

                but.Tag = realName;

                RelativePanel find = but.FindAscendantByName("REL") as RelativePanel;
                Image img = find.FindChildByName("ImagePreview") as Image;
                Button x = find.FindChildByName("DeleteImage") as Button;

                x.Tag = realName;
                img.Tag = realName;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new Uri(img.BaseUri, File.Path);

                img.Source = bitmapImage;

                img.Visibility = Visibility.Visible;
                x.Visibility = Visibility.Visible;

                but.IsEnabled = false;
            }
        }

        private async void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string strtag = button.Tag.ToString();

            int count = Int32.Parse(strtag);

            Grid find = button.Parent as Grid;

            var find2 = find.FindName("REL") as RelativePanel;

            var q = find2.FindName("quest") as TextBox;
            var a = find2.FindName("answer") as TextBox;
            Button btn1 = find2.FindName("AddPhoto") as Button;
            Button btn2 = find2.FindName("DeleteImage") as Button;
            Image img = find2.FindName("ImagePreview") as Image;

            if (img.Visibility == Visibility.Visible)
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("QImages");
                var file = await folder.GetFileAsync(btn2.Tag.ToString());
                await file.DeleteAsync();
            }
            q.Text = "";
            a.Text = "";
            btn1.Tag = "";
            btn2.Tag = "";
            btn1.IsEnabled = true;
            btn2.Visibility = Visibility.Collapsed;
            img.Source = null;
            img.Visibility = Visibility.Collapsed;

            EditBindModel.AddEditBinds.Remove(new AddEditBind() { ClickCount = count });
        }

        async void notifi()
        {
            IList<string> GetNames = GrabTitles("TitleTable", "TableName");
            IList<string> GetOfficial = GrabTitles("TitleTable", "title");

            int NameCount = GetNames.Count;

            //if (NameCount == 0)
            //{
            //    var exampleTaskName = "RepeatsNotificationTask";

            //    foreach (var task in BackgroundTaskRegistration.AllTasks)
            //    {
            //        if (task.Value.Name == exampleTaskName)
            //        {
            //            task.Value.Unregister(true);
            //            break;
            //        }
            //    }

            //    var tskName = "ToastBackgroundTask";
            //    foreach (var tsk in BackgroundTaskRegistration.AllTasks)
            //    {
            //        if (tsk.Value.Name == tskName)
            //        {
            //            tsk.Value.Unregister(true);
            //            break;
            //        }
            //    }

            //    Process.GetCurrentProcess().Kill();
            //}

            Random rnd = new Random();
            int r = rnd.Next(NameCount);

            string name = GetNames[r];
            string ofname = GetOfficial[r];

            IList<string> qu = GrabData(name, "question");
            IList<string> an = GrabData(name, "answer");
            IList<string> im = GrabData(name, "image");

            int ItemsCount = qu.Count;

            Random rnd2 = new Random();
            int r2 = rnd2.Next(ItemsCount);

            string question = qu[r2];
            string answer = an[r2];
            string image = im[r2];

            int conversationId = 384928;

            ToastVisual visual;

            if (image != "")
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("QImages");
                StorageFile img = await folder.GetFileAsync(image);

                string path = img.Path;

                visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = ofname
                            },

                            new AdaptiveText()
                            {
                                Text = question
                            },
                        },
                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = path
                        },

                        Attribution = new ToastGenericAttributionText()
                        {
                            Text = "Repeats (Beta)"
                        }
                    }
                };
            }
            else
            {
                visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = ofname
                            },

                            new AdaptiveText()
                            {
                                Text = question
                            },
                        },

                        Attribution = new ToastGenericAttributionText()
                        {
                            Text = "Repeats (Beta)"
                        }
                    }
                };
            }



            ToastActionsCustom actions = new ToastActionsCustom()
            {
                Inputs =
                {
                    new ToastTextBox("tbReply")
                    {
                        PlaceholderContent = "Tutaj wpisz odpowiedź"
                    }
                },

                Buttons =
                {
                    new ToastButton("Reply", answer)
                    {
                        ActivationType = ToastActivationType.Background,
                        TextBoxId = "tbReply"
                    }
                }
            };

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions,

                Launch = new QueryString()
                {
                    {"action", "viewQuestion" },
                    {"conversationId", conversationId.ToString() }
                }.ToString()
            };

            var toast = new ToastNotification(toastContent.GetXml());
            //toast.Tag = answer;

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static IList<string> GrabData(string FROM, string WHAT)
        {
            IList<string> data = new List<string>();
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT " + WHAT + " from " + FROM, db);
                SqliteDataReader query;
                //try
                //{
                query = selectCommand.ExecuteReader();
                //}
                //catch (SqliteException)
                //{
                //return data;
                //}
                while (query.Read())
                {
                    data.Add(query.GetString(0));
                }
                db.Close();
            }
            return data;
        }

        public static IList<string> GrabTitles(string FROM, string WHAT)
        {
            IList<string> titles = new List<string>();

            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT " + WHAT + " from " + FROM + " WHERE " + "IsEnabled='true'", db);
                SqliteDataReader query;
                //try
                //{
                query = selectCommand.ExecuteReader();
                //}
                //catch (SqliteException)
                //{
                //return data;
                //}
                while (query.Read())
                {
                    titles.Add(query.GetString(0));
                }
                db.Close();
            }
            return titles;
        }

    }
}
