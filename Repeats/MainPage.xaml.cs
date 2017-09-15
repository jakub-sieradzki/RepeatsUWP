using Repeats.Pages;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Repeats
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public static Frame FRAME;

        public MainPage()
        {
            this.InitializeComponent();

            LoadFOLDERS();

            var frame = new Frame();
            frame.ContentTransitions = new TransitionCollection();
            frame.ContentTransitions.Add(new NavigationThemeTransition());

            FRAME = MyFrame;

            MyFrame.Navigate(typeof(RepeatsList));
        }

        private async void LoadFOLDERS()
        {
            StorageFolder MainStorage = ApplicationData.Current.LocalFolder;
            StorageFolder b = await MainStorage.CreateFolderAsync("FOLDERS", CreationCollisionOption.OpenIfExists);
        }

        private void Repeat(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var main1 = loader.GetString("MAIN1");

            MyFrame.Navigate(typeof(RepeatsList));

            MAINtext.Text = main1;

            // add.Visibility = Visibility.Visible;
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var main2 = loader.GetString("MAIN2");

            MyFrame.Navigate(typeof(TestPage));

            MAINtext.Text = main2;

            // add.Visibility = Visibility.Visible;
        }

        private void WindowsInk(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var main3 = loader.GetString("MAIN3");

            MyFrame.Navigate(typeof(InkPage));
            MAINtext.Text = main3;
            add.Visibility = Visibility.Collapsed;
        }

        private void Settings(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var main4 = loader.GetString("MAIN4");

            MyFrame.Navigate(typeof(Settings), null, new DrillInNavigationTransitionInfo());

            MAINtext.Text = main4;

            // add.Visibility = Visibility.Visible;
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var main5 = loader.GetString("MAIN5");;

            MyFrame.Navigate(typeof(SearchListPage));
            MAINtext.Text = main5;

            // add.Visibility = Visibility.Visible;
        }
    }
}
