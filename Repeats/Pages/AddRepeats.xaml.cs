using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Background;
using Microsoft.QueryStringDotNET;
using System.Diagnostics;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;

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
        public int count;

        public AddRepeats()
        {
            this.InitializeComponent();

            Ring.IsActive = false;

            count = 0;

            this.ViewModel1 = new BindViewModel();

            ViewModel1.AddRepeat.Add(new bind1() { ClickCount = count });
        }

        public BindViewModel ViewModel1 { get; set; }

        private void Person_Entered(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            EditAvatar.Visibility = Visibility.Visible;
        }

        private void Person_Moved(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void Person_Exited(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            EditAvatar.Visibility = Visibility.Collapsed;
        }

        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {

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

            if(img.Visibility == Visibility.Visible)
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

            ViewModel1.AddRepeat.Remove(new bind1() { ClickCount = count });
        }

        public void NewItemClick(object sender, RoutedEventArgs e)
        {
            count++;
            ViewModel1.AddRepeat.Add(new bind1() { ClickCount = count });
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

                var date = DateTime.Now.ToString("yyyyMMddHHmmss");
                date = "I" + date;

                await File.RenameAsync(date + type, NameCollisionOption.GenerateUniqueName);

                but.Tag = date + type;

                RelativePanel find = but.FindAscendantByName("REL") as RelativePanel;
                Image img = find.FindChildByName("ImagePreview") as Image;
                Button x = find.FindChildByName("DeleteImage") as Button;

                x.Tag = date + type;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new Uri(img.BaseUri, File.Path);

                img.Source = bitmapImage;

                img.Visibility = Visibility.Visible;
                x.Visibility = Visibility.Visible;

                but.IsEnabled = false;
            }
        }

        private async void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            Button but = sender as Button;

            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("QImages");
            var file = await folder.GetFileAsync(but.Tag.ToString());
            await file.DeleteAsync();

            RelativePanel find = but.FindAscendantByName("REL") as RelativePanel;
            Image img = find.FindChildByName("ImagePreview") as Image;
            img.Visibility = Visibility.Collapsed;
            Button btn = find.FindChildByName("AddPhoto") as Button;

            btn.Tag = "";
            btn.IsEnabled = true;
            but.Visibility = Visibility.Collapsed;


        }

        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            Ring.Visibility = Visibility.Visible;
            Ring.IsActive = true;

            var findrelative = GRID.FindDescendants<RelativePanel>();
            var listrel = findrelative.ToList();
            int relcount = GRID.Items.Count;

            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            date = "R" + date;

            string realDate = DateTime.Now.ToShortDateString();
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
                    Button photoButton = panel.FindChildByName("AddPhoto") as Button;

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

                db.Close();
            }

            notifi();


            //AskTimeDialog TIME = new AskTimeDialog();
            //await TIME.ShowAsync();

            Frame.Navigate(typeof(RepeatsList));
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

            if(image != "")
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
    }
}
