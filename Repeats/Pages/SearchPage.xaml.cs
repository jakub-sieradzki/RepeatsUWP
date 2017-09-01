using System;
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
    public sealed partial class SearchPage : Page
    {
        string search;
        RelativePanel panel;
        TextBlock Question;
        TextBlock Answer;

        public SearchPage()
        {
            this.InitializeComponent();

            TEXT.Visibility = Visibility.Collapsed;
        }

        private void ItemsTemplate()
        {
            panel = new RelativePanel();

            Question = new TextBlock();

            Question.FontSize = 20;

            Answer = new TextBlock();

            Answer.FontSize = 20;
            Answer.Margin = new Thickness(0, 25, 0, 12);

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

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                TEXT.Visibility = Visibility.Collapsed;
            }
        }


        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                LIST.Items.Clear();

                TEXT.Visibility = Visibility.Collapsed;

                string NAME = SearchListPage.name;
                search = args.QueryText;

                StorageFolder main = ApplicationData.Current.LocalFolder;
                StorageFolder folder1 = await main.GetFolderAsync("FOLDERS");
                StorageFolder folder2 = await folder1.GetFolderAsync(NAME);
                StorageFile count = await folder2.GetFileAsync("ItemsCount.txt");

                string w = await FileIO.ReadTextAsync(count);

                string count1;

                using (StringReader reader = new StringReader(w))
                {
                    count1 = reader.ReadLine();
                }

                string Quest = "";
                string Answ = "";

                int x = Int32.Parse(count1);

                for (int i = 1; i <= x; i++)
                {
                    string txt = "header" + i.ToString() + ".txt";

                    StorageFile file = await folder2.GetFileAsync(txt);

                    string text = await FileIO.ReadTextAsync(file);

                    using (StringReader reader2 = new StringReader(text))
                    {
                        string FolderName = reader2.ReadLine();
                        Quest = reader2.ReadLine();
                        Answ = reader2.ReadLine();
                    }

                    if (Quest.Contains(args.QueryText) || Answ.Contains(args.QueryText))
                    {
                        ItemsTemplate();

                        Question.Text = Quest;
                        Answer.Text = Answ;
                    }
                }

                int list = LIST.Items.Count;

                if (list == 0)
                {
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    var strnot = loader.GetString("NothingSearch");

                    TEXT.Visibility = Visibility.Visible;
                    TEXT.Text = strnot;
                }
            }
            catch (Exception)
            {
                ExceptionUps();
            }

        }
    }
}
