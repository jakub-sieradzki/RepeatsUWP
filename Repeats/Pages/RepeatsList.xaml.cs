using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Windows.Storage;
using DataAccessLibrary;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RepeatsList : Page
    {
        public static string name;
        public static string OfficialName;
        public static bool IsEdit;
        public static string AV;
        public static StorageFolder folder;
        public static StorageFolder folder2;

        public RepeatsList()
        {
            this.InitializeComponent();

            GetFolder();

            this.ViewModel = new MainPageDataModel();
            IsEdit = false;

            if(ViewModel.Datas.Count == 0)
            {
                emptystack.Visibility = Visibility.Visible;
            }
        }

        public MainPageDataModel ViewModel { get; set; }

        async void GetFolder()
        {
            folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Images");
        }

        private void TakeTestButton(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            name = button.Tag.ToString();
            RelativePanel rel = button.FindAscendantByName("RelativeRepeatsList") as RelativePanel;
            OfficialName = rel.Tag.ToString();

            Frame.Navigate(typeof(TakeTestPage), null, new DrillInNavigationTransitionInfo());    
        }

        public void ItemClick_Click(object sender, ItemClickEventArgs e)
        {
            var data = (RepeatsListData)e.ClickedItem;
            var item = e.ClickedItem;
            var g = GridRepeats.PrepareConnectedAnimation("image", item, "Person");
            OfficialName = data.ProjectName;
            name = data.TableName;
            AV = data.avatarTag;
            IsEdit = true;
            Frame.Navigate(typeof(AddEditRepeats), null, new SuppressNavigationTransitionInfo());
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button del = sender as Button;
            string gettag = del.Tag.ToString();
            string imgtag = gettag;
            string IMGtag = imgtag.Replace("R", "I");
            string AVtag = imgtag.Replace("R", "A");

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var dialog1 = loader.GetString("deldialog1");
            var dialog2 = loader.GetString("deldialog2");
            var dialog3 = loader.GetString("deldialog3");
            var dialog4 = loader.GetString("deldialog4");

            ContentDialog delete = new ContentDialog
            {
                Title = dialog1,
                Content = dialog2,
                PrimaryButtonText = dialog3,
                CloseButtonText = dialog4
            };

            ContentDialogResult result = await delete.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.Datas.Remove(new RepeatsListData() { TableName = gettag });

                DataAccess.DropTable(gettag);
                DataAccess.DelFromTitleTable(gettag);

                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Images");
                var allimages = await folder.GetFilesAsync();
                var results = allimages.Where(x => x.Name.Contains(IMGtag));
                int count = results.Count();

                for(int i = 0; i < count; i++)
                {
                    var item = await folder.GetFileAsync(results.ElementAt(i).Name);
                    await item.DeleteAsync();
                }

                var avatars = allimages.Where(x => x.Name.Contains(AVtag));

                if(avatars.Count() != 0)
                {
                    var av = await folder.GetFileAsync(avatars.FirstOrDefault().Name);
                    await av.DeleteAsync();
                }       

                if (ViewModel.Datas.Count == 0)
                {
                    emptystack.Visibility = Visibility.Visible;
                }
            }
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddEditRepeats), null, new DrillInNavigationTransitionInfo());
        }
    }
}
