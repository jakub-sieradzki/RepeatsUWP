using System;
using System.IO;
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
    public sealed partial class Settings : Page
    {
        public static int time;

        public Settings()
        {
            this.InitializeComponent();

            Check();

            CheckTime();

            first.Click += TimeRadioButton_Checked;
            second.Click += TimeRadioButton_Checked;
            third.Click += TimeRadioButton_Checked;
            fourth.Click += TimeRadioButton_Checked;

            if (Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            {
                FEEDBACK.Visibility = Visibility.Visible;
            }
            else
            {
                FEEDBACK.Visibility = Visibility.Collapsed;
            }
        }

        private async void RATE(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"ms-windows-store://review/?ProductId=9NXLCWT9DQF2");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);
        }

        private async void CheckTime()
        {
            try
            {
                StorageFolder Storage = ApplicationData.Current.LocalFolder;

                if(await Storage.TryGetItemAsync("time.txt") != null)
                {
                StorageFile Storage2 = await Storage.GetFileAsync("time.txt");

                string t = await FileIO.ReadTextAsync(Storage2);

                string time;

                using (StringReader reader = new StringReader(t))
                {
                    time = reader.ReadLine();
                }

                int b = Int32.Parse(time);

                if (b == 15)
                {
                    first.IsChecked = true;
                }
                else if (b == 30)
                {
                    second.IsChecked = true;
                }
                else if (b == 45)
                {
                    third.IsChecked = true;
                }
                else if (b == 60)
                {
                    fourth.IsChecked = true;
                }

                }
                else
                {

                }
            }
            catch (Exception)
            {
                ExceptionUps();
            }
        }

        private void Check()
        {
            try
            {
                var taskName = "RepeatsNotificationTask";

                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {
                    if (cur.Value.Name == taskName)
                    {
                        turnoff.Visibility = Visibility.Visible;
                        turnon.Visibility = Visibility.Collapsed;
                        TEXT.Visibility = Visibility.Visible;
                        stack.Visibility = Visibility.Visible;
                        break;
                    }
                    else
                    {
                        turnoff.Visibility = Visibility.Collapsed;
                        turnon.Visibility = Visibility.Visible;
                        TEXT.Visibility = Visibility.Collapsed;
                        stack.Visibility = Visibility.Collapsed;
                    }
                }

                int it = RepeatsList.items;

                if (it == 0)
                {
                    turnon.IsEnabled = false;
                }
            }
            catch
            {
                ExceptionUps();
            }

        }

        public static void CancelTask()
        {
            try
            {
                var taskName = "RepeatsNotificationTask";

                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {
                    if (cur.Value.Name == taskName)
                    {
                        cur.Value.Unregister(true);
                    }
                }
            }
            catch (Exception)
            {
                ExceptionUps();
            }

        }

        private async void SAVE()
        {
            try
            {
                StorageFolder Storage = ApplicationData.Current.LocalFolder;
                StorageFile Storage2 = await Storage.GetFileAsync("time.txt");
                StorageFile Storage1 = await Storage.CreateFileAsync("time.txt", CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteTextAsync(Storage2, time.ToString());

                CancelTask();

                RegisterTask.RegisterBackgroundTask();
            }
            catch (Exception)
            {
                ExceptionUps();
            }


        }

        private void TimeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rb = sender as RadioButton;
                string a = rb.Tag.ToString();
                switch (a)
                {
                    case "15":
                        time = 15;
                        SAVE();
                        break;
                    case "30":
                        time = 30;
                        SAVE();
                        break;
                    case "45":
                        time = 45;
                        SAVE();
                        break;
                    case "60":
                        time = 60;
                        SAVE();
                        break;
                }
            }
            catch(Exception)
            {
                ExceptionUps();
            }

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

        private void CancelTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var stratt11 = loader.GetString("off");

                CancelTask();
                turnoff.Content = stratt11;
                turnoff.IsEnabled = false;
            }
            catch (Exception)
            {
                ExceptionUps();
            }
        }

        private async void Feedback_Click(object sender, RoutedEventArgs e)
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        private void RegisterTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var taskName = "RepeatsNotificationTask";

                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {
                    if (cur.Value.Name == taskName)
                    {

                    }
                    else
                    {
                        var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                        var on1 = loader.GetString("ON");

                        RegisterTask.RegisterBackgroundTask();
                        turnon.IsEnabled = false;
                        turnon.Content = on1;
                    }
                }
            }
            catch (Exception)
            {
                ExceptionUps();
            }

        }
    }
}
