using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using DataAccessLibrary;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public class StreamBind
    {
        public StorageFile File { get; set; }
    }

    public class StreamBindModel
    {
        private ObservableCollection<StreamBind> streamBinds = new ObservableCollection<StreamBind>();
        public ObservableCollection<StreamBind> StreamBinds { get { return this.streamBinds; } }
        public StreamBindModel()
        {

        }
    }

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
        public List<string> ImageName;
        public StorageFile AvatarFile;
        public bool AvatarChanged;
        public BitmapImage BITMAPImage;
        public string ListAV;
        public List<string> REMOVEimagewithQ;

        public AddEditRepeats()
        {
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;

            this.InitializeComponent();

            REMOVEimagewithQ = new List<string>();
            ImageName = new List<string>();
            this.EditBindModel = new AddEditBindModel();
            this.StreamBindModel = new StreamBindModel();

            AvatarChanged = false;

            date = DateTime.Now.ToString("yyyyMMddHHmmss");
            date = "R" + date;

            realDate = DateTime.Now.ToShortDateString();

            Count = 0;

            edit = RepeatsList.IsEdit;

            BITMAPImage = new BitmapImage();
            BITMAPImage.UriSource = new Uri("ms-appx:///Assets/new logo2.png");


            Pic.ProfilePicture = BITMAPImage;

            if (edit)
            {
                ConnectedAnimation imageAnimation =
                    ConnectedAnimationService.GetForCurrentView().GetAnimation("image");
                if (imageAnimation != null)
                {
                    imageAnimation.TryStart(Pic);
                }

                tablename = RepeatsList.name;
                date = RepeatsList.name;
                string name = RepeatsList.OfficialName;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new Uri(RepeatsList.AV);

                Pic.ProfilePicture = bitmapImage;

                AskName.Text = name;

                PicRel.Tag = RepeatsList.AV;

                List<string> questions = DataAccess.GrabData(tablename, "question");
                List<string> answers = DataAccess.GrabData(tablename, "answer");
                List<string> images = DataAccess.GrabData(tablename, "image");

                int count = questions.Count;

                for (int i = 0; i < count; i++)
                {
                    string QUESTION = questions.ElementAt(i);
                    string ANSWER = answers.ElementAt(i);
                    string IMAGE = images.ElementAt(i);

                    if (IMAGE == "")
                    {
                        EditBindModel.AddEditBinds.Add(new AddEditBind()
                        {
                            ClickCount = Count,
                            GetQuestion = QUESTION,
                            GetAnswer = ANSWER,
                            GetImage = "",
                            visibility = Visibility.Collapsed,
                            enabled = true,
                            ImageTag = ""
                        });
                    }
                    else
                    {
                        EditBindModel.AddEditBinds.Add(new AddEditBind()
                        {
                            ClickCount = Count,
                            GetQuestion = QUESTION,
                            GetAnswer = ANSWER,
                            GetImage = RepeatsList.folder.Path + "\\" + IMAGE,
                            visibility = Visibility.Visible,
                            enabled = false,
                            ImageTag = IMAGE
                        });
                    }

                    Count++;
                }
            }
            else
            {
                EditBindModel.AddEditBinds.Add(new AddEditBind()
                {
                    ClickCount = Count,
                    visibility = Visibility.Collapsed,
                    enabled = true,
                    ImageTag = "",
                    GetImage = ""
                });

                Count++;

                Pic.ProfilePicture = BITMAPImage;
                PicRel.Tag = "ms-appx:///Assets/new logo2.png";
            }
        }

        public AddEditBindModel EditBindModel { get; set; }
        public StreamBindModel StreamBindModel { get; set; }

        private void NewItemClick(object sender, RoutedEventArgs e)
        {
            EditBindModel.AddEditBinds.Add(new AddEditBind()
            {
                ClickCount = Count,
                enabled = true,
                visibility = Visibility.Collapsed,
                ImageTag = ""
            });

            Count++;
        }

        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Images");

            if (edit)
            {
                DataAccess.DropTable(tablename);
                DataAccess.DelFromTitleTable(tablename);
            }

            int ImagesToRemoveDEL = 0;

            Ring.Visibility = Visibility.Visible;
            Ring.IsActive = true;

            var findrelative = GRID.FindDescendants<RelativePanel>();
            var listrel = findrelative.ToList();
            int relcount = GRID.Items.Count;

            string getname = AskName.Text;

            DataAccess.CreateTable(date);

            string Date = date.Replace("R", "I");
            int fileindex = 0;
            for (int i = 1; i <= relcount; i++)
            {
                RelativePanel panel = listrel[i];
                string tag = "";
                TextBox questbox = panel.FindChildByName("quest") as TextBox;
                TextBox answerbox = panel.FindChildByName("answer") as TextBox;
                Image photoImage = panel.FindChildByName("ImagePreview") as Image;
                Button x = panel.FindChildByName("DeleteImage") as Button;
                Button DEL = panel.FindChildByName("DELButton") as Button;
                string TAG = x.Tag.ToString();
                if (photoImage.Tag.ToString() == "T")
                {
                    StorageFile file = StreamBindModel.StreamBinds[fileindex].File;

                    StorageFile CopyFile = await file.CopyAsync(folder);
                    await CopyFile.RenameAsync(Date + file.FileType, NameCollisionOption.GenerateUniqueName);
                    tag = CopyFile.Name;
                    fileindex++;
                }
                else if (photoImage.Tag.ToString() != "")
                {
                    tag = photoImage.Tag.ToString();
                }

                if (TAG == "D" && edit)
                {
                    StorageFile file = await folder.GetFileAsync(ImageName.ElementAt(ImagesToRemoveDEL));
                    await file.DeleteAsync();
                    ImagesToRemoveDEL++;
                }

                string question = questbox.Text;
                string answer = answerbox.Text;

                DataAccess.SaveToTableSet(question, answer, tag, date);
            }

            string TA = Pic.Tag.ToString();
            BitmapImage bit = Pic.ProfilePicture as BitmapImage;
            string TAGPIC = "";

            if (TA == "T")
            {
                string type = AvatarFile.FileType;
                StorageFile File = await AvatarFile.CopyAsync(folder);

                string DATE = date.Replace("R", "A");

                await File.RenameAsync(DATE + type, NameCollisionOption.GenerateUniqueName);

                TAGPIC = File.Path;

                if (AvatarChanged)
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(RepeatsList.AV);
                    await file.DeleteAsync();
                }
            }
            else
            {
                TAGPIC = RepeatsList.AV;
            }

            if(TAGPIC == null)
            {
                TAGPIC = BITMAPImage.UriSource.ToString();
            }



            DataAccess.SaveToTitleTable(getname, date, realDate, TAGPIC);

            bool taskRegistered = false;
            var exampleTaskName = "RepeatsNotificationTask";

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == exampleTaskName)
                {
                    taskRegistered = true;
                    break;
                }
            }

            if (!taskRegistered)
            {
                AskTimeDialog TIME = new AskTimeDialog();
                await TIME.ShowAsync();
            }

            int C = REMOVEimagewithQ.Count;

            for(int i = 0; i < C; i++)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(REMOVEimagewithQ.ElementAt(i));
                await file.DeleteAsync();
            }

            Frame.Navigate(typeof(RepeatsList));
        }

        private async void ChangeAvatar_Click(object sender, RoutedEventArgs e)
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
                BitmapImage bitmap = Pic.ProfilePicture as BitmapImage; 
                if (bitmap.UriSource != BITMAPImage.UriSource)
                {
                    AvatarChanged = true;
                }
                Pic.Tag = "T";

                AvatarFile = file;

                IRandomAccessStream Stream = await file.OpenAsync(FileAccessMode.Read);

                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(Stream);

                Pic.ProfilePicture = bitmapImage;
            }
        }

        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            Button but = sender as Button;
            string tag = but.Tag.ToString();

            RelativePanel find = but.FindAscendantByName("REL") as RelativePanel;
            Image img = find.FindChildByName("ImagePreview") as Image;
            img.Visibility = Visibility.Collapsed;
            Button btn = find.FindChildByName("AddPhoto") as Button;

            ImageName.Add(img.Tag.ToString());
            but.Tag = "D";
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
                RelativePanel find = but.FindAscendantByName("REL") as RelativePanel;
                Image img = find.FindChildByName("ImagePreview") as Image;
                Button x = find.FindChildByName("DeleteImage") as Button;

                IRandomAccessStream STREAM = await file.OpenAsync(FileAccessMode.Read);

                StreamBindModel.StreamBinds.Add(new StreamBind { File = file });

                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(STREAM);

                img.Source = bitmapImage;

                img.Tag = "T";

                img.Visibility = Visibility.Visible;
                x.Visibility = Visibility.Visible;

                but.IsEnabled = false;
            }
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            if (EditBindModel.AddEditBinds.Count != 1)
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

                if (img.Source != null)
                {
                    BitmapImage bitmapImage = img.Source as BitmapImage;
                    string s = bitmapImage.UriSource.ToString();
                    REMOVEimagewithQ.Add(s);
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
        }
    }
}
