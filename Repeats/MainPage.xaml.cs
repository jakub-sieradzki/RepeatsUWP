using Repeats.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// 

    public class SearchBind
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string SetName { get; set; }
        public string SetTable { get; set; }
    }

    public class SearchBindModel
    {
        private ObservableCollection<SearchBind> search = new ObservableCollection<SearchBind>();
        public ObservableCollection<SearchBind> Search { get { return this.search; } }
        public SearchBindModel()
        {
            List<string> Oname = GetFromDB.GrabData("TitleTable", "title");
            List<string> name = GetFromDB.GrabData("TitleTable", "TableName");

            int count = name.Count;

            for (int i = 0; i < count; i++)
            {
                List<string> Questions = GetFromDB.GrabData(name.ElementAt(i), "question");
                List<string> Answers = GetFromDB.GrabData(name.ElementAt(i), "answer");

                int All = Questions.Count;

                for(int j = 0; j < All; j++)
                {
                    this.search.Add(new SearchBind() { Question = "Q: " + Questions.ElementAt(j), Answer = "A: " + Answers.ElementAt(j), SetName = Oname.ElementAt(i), SetTable = name.ElementAt(i) });
                }
            }
        }
    }

    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public static Frame FRAME;

        public MainPage()
        {
            this.InitializeComponent();

            //var frame = new Frame();
            //frame.ContentTransitions = new TransitionCollection();
            //frame.ContentTransitions.Add(new NavigationThemeTransition());

            FRAME = ContentFrame;

            CreateFolder();

            ContentFrame.Navigate(typeof(RepeatsList), null, new SuppressNavigationTransitionInfo());
        }

        public SearchBindModel BindModel { get; set; }


        private void ASB_Focus(object sender, RoutedEventArgs e)
        {
            this.BindModel = new SearchBindModel();
        }

        async void CreateFolder()
        {
            StorageFolder MainStorage = ApplicationData.Current.LocalFolder;
            await MainStorage.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
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
                    {typeof(InkPage), "ink"},
                    {typeof(TakeTestPage), "taketest" },
                    {typeof(TestResults), "testresults" },
                    {typeof(AddEditRepeats), "addeditrepeats" }
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

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = BindModel.Search.Where(x=>x.Question.Contains(sender.Text, StringComparison.CurrentCultureIgnoreCase) || x.Answer.Contains(sender.Text, StringComparison.CurrentCultureIgnoreCase));
            }
        }


        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string
            sender.Text = "";
        }


        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
            }
            else
            {
                // Use args.QueryText to determine what to do.
            }
        }
    }
}
