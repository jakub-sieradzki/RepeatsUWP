using System;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddRepeats : Page
    {
        public static int c;
        public static int time;
        public string question;
        public string name;
        public string txt;
        public TextBox Question;
        public TextBox Answer;
        public StorageFolder MainStorage;
        RelativePanel panel;

        public AddRepeats()
        {
            this.InitializeComponent();

            DELETE.Visibility = Visibility.Collapsed;

            c = 0;

            Ring.IsActive = false;

            ItemsTemplate();

            STORAGE();
        }

        private void STORAGE()
        {
            MainStorage = ApplicationData.Current.LocalFolder;
        }

        private void ItemsTemplate()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var strque = loader.GetString("Question");
            var strans = loader.GetString("Answer");

            c++;

            panel = new RelativePanel();
            panel.Name = "P" + c.ToString();

            Question = new TextBox();

            Question.Name = "Q" + c.ToString();
            Question.PlaceholderText = strque;
            Question.FontSize = 20;

            Answer = new TextBox();

            Answer.Name = "A" + c.ToString();
            Answer.PlaceholderText = strans;
            Answer.FontSize = 20;
            Answer.Margin = new Thickness(0, 40, 0, 0);

            var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;

            if (qualifiers["DeviceFamily"] == "DeviceFamily-Mobile")
            {
                Question.MinWidth = 140;
                Answer.MinWidth = 140;
            }
            else
            {
                Question.MinWidth = 300;
                Answer.MinWidth = 300;
            }

            panel.Children.Add(Question);
            panel.Children.Add(Answer);

            LIST.Items.Add(panel);
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            LIST.Items.Remove(LIST.SelectedItem);
            DELETE.Visibility = Visibility.Collapsed;
        }

        public void NewItemClick(object sender, RoutedEventArgs e)
        {
            ItemsTemplate();
        }

        public void Item_Click(object sender, ItemClickEventArgs e)
        {
            DELETE.Visibility = Visibility.Visible;
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
                    DELETE.IsEnabled = true;
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

        private async void AcceptClick(object sender, RoutedEventArgs e)
        {
            Ring.IsActive = true;
            Add.IsEnabled = false;
            DELETE.IsEnabled = false;
            Save.IsEnabled = false;
            try
            {
                int NEWint = 0;

                int d = LIST.Items.Count;

                if (d == 0 || d == 1)
                {
                    Ring.IsActive = false;
                    Add.IsEnabled = true;
                    DELETE.IsEnabled = true;
                    Save.IsEnabled = true;

                    CountDialog();
                }
                else
                {
                    string n = AskNameDialog.name;

                    StorageFolder c = await MainStorage.GetFolderAsync("FOLDERS");
                    StorageFolder sampleFolder = await c.CreateFolderAsync(n, CreationCollisionOption.ReplaceExisting);
                    StorageFolder FOLDER = await c.GetFolderAsync(n);

                    StorageFile sampleFile = await FOLDER.CreateFileAsync("ItemsCount.txt", CreationCollisionOption.ReplaceExisting);
                    StorageFile sampleFile2 = await FOLDER.GetFileAsync("ItemsCount.txt");
                    await FileIO.WriteTextAsync(sampleFile2, d.ToString());

                    int COUNTS2 = LIST.Items.Count;

                    for (int i = 1; i <= COUNTS2; i++)
                    {
                        object FindRel = LIST.FindName("P" + i.ToString());

                        if (FindRel == null)
                        {
                            COUNTS2 += 1;
                        }
                        else
                        {
                            NEWint++;

                            RelativePanel Rel = (RelativePanel)FindRel;

                            object box1 = Rel.FindName("Q" + i.ToString());
                            object box2 = Rel.FindName("A" + i.ToString());

                            TextBox txtb = (TextBox)box1;
                            string Q = txtb.Text;

                            TextBox txtb2 = (TextBox)box2;
                            string A = txtb2.Text;

                            if (Q == "" || A == "")
                            {
                                Ring.IsActive = false;
                                Add.IsEnabled = true;
                                DELETE.IsEnabled = true;
                                Save.IsEnabled = true;

                                WriteDialog();
                                break;
                            }
                            else
                            {
                                txt = "header" + NEWint.ToString() + ".txt";

                                StorageFile sampleFile10 = await FOLDER.CreateFileAsync(txt, CreationCollisionOption.ReplaceExisting);

                                string[] lines = { n, Q, A };

                                StorageFile sampleFile20 = await FOLDER.GetFileAsync(txt);
                                await FileIO.WriteLinesAsync(sampleFile20, lines);

                                if (i == COUNTS2)
                                {
                                    ASKTIME();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                ExceptionUps();

                Ring.IsActive = false;

                Add.IsEnabled = true;
                DELETE.IsEnabled = true;
                Save.IsEnabled = true;
            }
        }
    }
}
