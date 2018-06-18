using Repeats.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

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

            FRAME = ContentFrame;

            ContentFrame.Navigate(typeof(RepeatsList));
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem 
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "repeats")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }

            ContentFrame.Navigated += On_Navigated;

            // add keyboard accelerators for backwards navigation
            KeyboardAccelerator GoBack = new KeyboardAccelerator();
            GoBack.Key = VirtualKey.GoBack;
            GoBack.Invoked += BackInvoked;
            KeyboardAccelerator AltLeft = new KeyboardAccelerator();
            AltLeft.Key = VirtualKey.Left;
            AltLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(GoBack);
            this.KeyboardAccelerators.Add(AltLeft);
            // ALT routes here
            AltLeft.Modifiers = VirtualKeyModifiers.Menu;

        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(Settings));
            }
            else
            {
                // find NavigationViewItem with Content that equals InvokedItem
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as NavigationViewItem);
            }
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "repeats":
                    ContentFrame.Navigate(typeof(RepeatsList));
                    break;

                case "test":
                    ContentFrame.Navigate(typeof(TestPage));
                    break;

                case "ink":
                    ContentFrame.Navigate(typeof(InkPage));
                    break;
            }
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private bool On_BackRequested()
        {
            bool navigated = false;

            // don't go back if the nav pane is overlayed
            if (NavView.IsPaneOpen && (NavView.DisplayMode == NavigationViewDisplayMode.Compact || NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
            {
                return false;
            }
            else
            {
                if (ContentFrame.CanGoBack)
                {
                    ContentFrame.GoBack();
                    navigated = true;
                }
            }
            return navigated;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(Settings))
            {
                NavView.SelectedItem = NavView.SettingsItem as NavigationViewItem;
            }
            else
            {
                Dictionary<Type, string> lookup = new Dictionary<Type, string>()
                {
                    {typeof(RepeatsList), "repeats"},
                    {typeof(TestPage), "test"},
                    {typeof(InkPage), "ink"},
                    {typeof(AddRepeats), "addrepeats" },
                    {typeof(EditItems), "edit"},
                    {typeof(TakeTestPage), "taketest" }
                };

                String stringTag = lookup[ContentFrame.SourcePageType];

                // set the new SelectedItem  
                foreach (NavigationViewItemBase item in NavView.MenuItems)
                {
                    if (item is NavigationViewItem && item.Tag.Equals(stringTag))
                    {
                        item.IsSelected = true;
                        break;
                    }
                }
            }
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

            //MyFrame.Navigate(typeof(RepeatsList));

            //MAINtext.Text = main1;

            // add.Visibility = Visibility.Visible;
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var main2 = loader.GetString("MAIN2");

            //MyFrame.Navigate(typeof(TestPage));

            //MAINtext.Text = main2;

            // add.Visibility = Visibility.Visible;
        }

        private void WindowsInk(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var main3 = loader.GetString("MAIN3");

            //MyFrame.Navigate(typeof(InkPage));
            //MAINtext.Text = main3;
            // add.Visibility = Visibility.Collapsed;
        }

        private void Settings(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var main4 = loader.GetString("MAIN4");

            //MyFrame.Navigate(typeof(Settings), null, new DrillInNavigationTransitionInfo());

            //MAINtext.Text = main4;

            // add.Visibility = Visibility.Visible;
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var main5 = loader.GetString("MAIN5");;

            //MyFrame.Navigate(typeof(SearchListPage));
            //MAINtext.Text = main5;

            // add.Visibility = Visibility.Visible;
        }
    }
}
