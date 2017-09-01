using System;
using System.IO;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TakeTestPage : Page
    {
        public int item;
        public int GOOD;
        public int BAD;
        public string QUESTION;
        public string ANSWER;
        public string NAME;

        public TakeTestPage()
        {
            this.InitializeComponent();

            NAME = TestPage.name;
            item = 1;

            GOOD = 0;
            BAD = 0;

            GetItemsFromFolder();
            LoadLocalItems();
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


        public async void GetItemsFromFolder()
        {
            try
            {
                StorageFolder MainStorage = ApplicationData.Current.LocalFolder;
                StorageFolder Fol = await MainStorage.GetFolderAsync("FOLDERS");
                StorageFolder Subfolder = await Fol.GetFolderAsync(NAME);

                StorageFile Items = await Subfolder.GetFileAsync("ItemsCount.txt");
                string w = await FileIO.ReadTextAsync(Items);

                string count;

                using (StringReader reader = new StringReader(w))
                {
                    count = reader.ReadLine();
                }

                int COUNTS = Int32.Parse(count);

                if (item <= COUNTS)
                {
                    StorageFile Headers = await Subfolder.GetFileAsync("header" + item.ToString() + ".txt");

                    string read = await FileIO.ReadTextAsync(Headers);

                    string FolderName;

                    using (StringReader Reader = new StringReader(read))
                    {
                        FolderName = Reader.ReadLine();
                        QUESTION = Reader.ReadLine();
                        ANSWER = Reader.ReadLine();
                    }

                    TextFlip1.Text = QUESTION;
                }
                else
                {
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    var strtest1 = loader.GetString("TEST1");
                    var strtest2 = loader.GetString("TEST2");
                    var strtest3 = loader.GetString("TEST3");



                    BoxFlip1.Visibility = Visibility.Collapsed;
                    ButtonFlip1.Visibility = Visibility.Collapsed;

                    string all = strtest1 + COUNTS.ToString() + Environment.NewLine + Environment.NewLine + strtest2 + GOOD.ToString() + Environment.NewLine + strtest3 + BAD.ToString();

                    TextFlip1.Text = all;
                }
                item += 1;
            }
            catch (Exception)
            {
                ExceptionUps();
            }

        }

        private void LoadLocalItems()
        {
            Button2.Visibility = Visibility.Collapsed;
            StackFlip1.Visibility = Visibility.Collapsed;
            TEXT3.Visibility = Visibility.Collapsed;

            BoxFlip1.IsEnabled = true;
            ButtonFlip1.IsEnabled = true;

            BoxFlip1.BorderBrush = new SolidColorBrush(Colors.Gray);
            BoxFlip1.Text = "";
        }

        private void CheckAnswer()
        {
            try
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var strgoo = loader.GetString("Good");
                var strbad = loader.GetString("Bad");
                var strfin = loader.GetString("Correct");


                if (BoxFlip1.Text == ANSWER)
                {
                    Stack1.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    Stack1.Text = "\uE001";
                    Stack1.Foreground = new SolidColorBrush(Colors.Green);
                    Stack2.Text = strgoo;

                    StackFlip1.Visibility = Visibility.Visible;
                    Button2.Visibility = Visibility.Visible;

                    GOOD += 1;
                }
                else
                {
                    Stack1.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    Stack1.Text = "\uE10A";
                    Stack1.Foreground = new SolidColorBrush(Colors.Red);
                    Stack2.Text = strbad;
                    StackFlip1.Visibility = Visibility.Visible;
                    Button2.Visibility = Visibility.Visible;
                    TEXT3.Text = strfin + ANSWER;
                    TEXT3.Visibility = Visibility.Visible;

                    BAD += 1;
                }
            }
            catch (Exception)
            {
                ExceptionUps();
            }

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            GetItemsFromFolder();
            LoadLocalItems();
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            CheckAnswer();

            BoxFlip1.IsEnabled = false;
            ButtonFlip1.IsEnabled = false;
        }
    }
}
