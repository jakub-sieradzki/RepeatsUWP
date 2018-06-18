using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditItems : Page
    {
        public static string FolderName;
        public TextBox Question;
        public TextBox Answer;
        RelativePanel panel;
        int c;

        public EditItems()
        {
            this.InitializeComponent();

            DELETE.Visibility = Visibility.Collapsed;

            Ring.IsActive = false;

            c = 0;

            Load();
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
            Question.Text = strque;
            Question.FontSize = 20;

            Answer = new TextBox();

            Answer.Name = "A" + c.ToString();
            Answer.Text = strans;
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

            LISTItems.Items.Add(panel);
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

        //private List<string> GrabQuestions()
        //{
        //    List<string> questions = new List<string>();
        //    using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
        //    {
        //        db.Open();
        //        SqliteCommand selectCommand = new SqliteCommand("SELECT question from " + NAME, db);
        //        SqliteDataReader query;
        //        try
        //        {
        //            query = selectCommand.ExecuteReader();
        //        }
        //        catch (SqliteException error)
        //        {
        //            //Handle error
        //            return questions;
        //        }
        //        while (query.Read())
        //        {
        //            questions.Add(query.GetString(0));
        //        }
        //        db.Close();
        //    }
        //    return questions;
        //}

        private async void Load()
        {
            try
            {
                string NAME = RepeatsList.name;


                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFolder p = await storageFolder.GetFolderAsync("FOLDERS");
                StorageFolder d = await p.GetFolderAsync(NAME);

                StorageFile sampleFile2 = await d.GetFileAsync("ItemsCount.txt");

                string w = await FileIO.ReadTextAsync(sampleFile2);

                string count1;

                using (StringReader reader = new StringReader(w))
                {
                    count1 = reader.ReadLine();
                }

                int x = Int32.Parse(count1);

                for (int i = 1; i <= x; i++)
                {
                    ItemsTemplate();

                    var S = await d.GetFileAsync("header" + i.ToString() + ".txt");

                    string Strings = await FileIO.ReadTextAsync(S);

                    string Quest;
                    string Answ;

                    using (StringReader reader2 = new StringReader(Strings))
                    {
                        FolderName = reader2.ReadLine();
                        Quest = reader2.ReadLine();
                        Answ = reader2.ReadLine();
                    }

                    Question.Text = Quest;
                    Answer.Text = Answ;

                }
            }
            catch (Exception)
            {
                ExceptionUps();
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

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            Ring.IsActive = true;
            ADD.IsEnabled = false;
            DELETE.IsEnabled = false;
            SAVE.IsEnabled = false;

            try
            {
                int COUNTS1 = LISTItems.Items.Count;

                if (COUNTS1 == 0 || COUNTS1 == 1)
                {
                    Ring.IsActive = false;
                    ADD.IsEnabled = true;
                    DELETE.IsEnabled = true;
                    SAVE.IsEnabled = true;

                    CountDialog();
                }
                else
                {
                    int NEWint = 0;

                    string NAME = RepeatsList.name;

                    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                    StorageFolder p = await storageFolder.GetFolderAsync("FOLDERS");
                    StorageFolder d = await p.GetFolderAsync(NAME);

                    StorageFile sampleFile2 = await d.GetFileAsync("ItemsCount.txt");

                    string w = await FileIO.ReadTextAsync(sampleFile2);

                    string count1;

                    using (StringReader reader = new StringReader(w))
                    {
                        count1 = reader.ReadLine();
                    }

                    int x = Int32.Parse(count1);
                    for (int g = 1; g <= x; g++)
                    {
                        string txt = "header" + g.ToString() + ".txt";

                        if (await d.TryGetItemAsync(txt) == null)
                        {
                            continue;
                        }
                        else
                        {
                            StorageFile sampleFile20 = await d.GetFileAsync(txt);

                            await sampleFile20.DeleteAsync();
                        }
                    }

                    StorageFile tyty = await d.CreateFileAsync("ItemsCount.txt", CreationCollisionOption.ReplaceExisting);
                    StorageFile co = await d.GetFileAsync("ItemsCount.txt");

                    await FileIO.WriteTextAsync(co, COUNTS1.ToString());

                    string n = RepeatsList.name;

                    int COUNTS2 = LISTItems.Items.Count;

                    for (int i = 1; i <= COUNTS2; i++)
                    {
                        object FindRel = LISTItems.FindName("P" + i.ToString());

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
                                ADD.IsEnabled = true;
                                DELETE.IsEnabled = true;
                                SAVE.IsEnabled = true;

                                WriteDialog();
                                break;
                            }
                            else
                            {
                                string txt2 = "header" + NEWint.ToString() + ".txt";

                                StorageFile sampleFile10 = await d.CreateFileAsync(txt2, CreationCollisionOption.ReplaceExisting);

                                string[] lines = { n, Q, A };

                                StorageFile sampleFile20 = await d.GetFileAsync(txt2);
                                await FileIO.WriteLinesAsync(sampleFile20, lines);

                                if (i == COUNTS2)
                                {
                                    Frame.Navigate(typeof(RepeatsList));
                                }
                            }
                        }
                    }
                    Ring.IsActive = false;
                }
            }
            catch (Exception)
            {
                ExceptionUps();

                Ring.IsActive = false;

                ADD.IsEnabled = true;
                DELETE.IsEnabled = true;
                SAVE.IsEnabled = true;
            }
        }

        private void Item_Click(object sender, ItemClickEventArgs e)
        {
            DELETE.Visibility = Visibility.Visible;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            LISTItems.Items.Remove(LISTItems.SelectedItem);
            DELETE.Visibility = Visibility.Collapsed;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ItemsTemplate();
        }
    }
}
