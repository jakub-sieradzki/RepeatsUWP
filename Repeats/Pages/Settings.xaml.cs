using Microsoft.Data.Sqlite;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public class bind3
    {
        public string SetName { get; set; }
        public string TName { get; set; }
        public bool ISON { get; set; }
    }

    public class BindSettingsViewModel
    {
        private ObservableCollection<bind3> setRepeat = new ObservableCollection<bind3>();
        public ObservableCollection<bind3> SetRepeat { get { return this.setRepeat; } }
        public BindSettingsViewModel()
        {
            List<string> Grab_Titles = GetFromDB.GrabData("TitleTable", "title");
            List<string> Grab_Table = GetFromDB.GrabData("TitleTable", "TableName");
            List<string> Grab_Enabled = GetFromDB.GrabData(" TitleTable", "IsEnabled");

            int count = Grab_Titles.Count;

            for(int i = 0; i < count; i++)
            {
                this.setRepeat.Add(new bind3() { SetName = Grab_Titles.ElementAt(i), TName = Grab_Table.ElementAt(i), ISON = Boolean.Parse(Grab_Enabled.ElementAt(i))});
            }
        }  
    }


    public sealed partial class Settings : Page
    {
        public static int time;
        public static bool TimeOnly;
        public string language;
        public BitmapImage bitmaplight;
        public BitmapImage bitmapdark;

        public Settings()
        {
            this.InitializeComponent();

            BitmapImage bitmapdark = new BitmapImage();
            bitmapdark.UriSource = new Uri("ms-appx:///Assets/Repeats-logo-extended-dark.png");

            BitmapImage bitmaplight = new BitmapImage();
            bitmaplight.UriSource = new Uri("ms-appx:///Assets/Repeats-logo-extended.png");

            var ci = CultureInfo.InstalledUICulture;
            language = ci.Name;

            if(ActualTheme == ElementTheme.Light)
            {
                Logo.Source = bitmapdark;
            }
            else if(ActualTheme == ElementTheme.Dark)
            {
                Logo.Source = bitmaplight;
            }

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var vs = loader.GetString("Version");

            string ApplicationVersion = $"{SystemInformation.ApplicationVersion.Major}.{SystemInformation.ApplicationVersion.Minor}.{SystemInformation.ApplicationVersion.Build}";

            InfoBlock.Text = vs + " " + ApplicationVersion + Environment.NewLine + Environment.NewLine + "Developer: Jakub Sieradzki";
            TimeOnly = false;
            var taskRegistered = false;
            var exampleTaskName = "RepeatsNotificationTask";

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == exampleTaskName)
                {
                    taskRegistered = true;
                    break;
                }
            }

            if (taskRegistered)
            {
                Switch.IsOn = true;
            }
            else
            {
                if (language == "pl-PL")
                {
                    Time.Text = "Powiadomienia przychodzą co 0 minut";
                }
                else
                {
                    Time.Text = "Notifications are sent every 0 minutes";
                }

                Switch.IsOn = false;
                LISTofSets.IsEnabled = false;
                ChangeButton.IsEnabled = false;
            }

            Switch.Toggled += Switch_Toggled;

            this.VIEWMODEL = new BindSettingsViewModel();

            GetTime();

            if(VIEWMODEL.SetRepeat.Count == 0)
            {
                RESETbutton.IsEnabled = false;
                Switch.IsEnabled = false;
            }

            if (!Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            {
                FEEDBACK.Visibility = Visibility.Collapsed;
            }
        }

        void GetTime()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            object value = localSettings.Values["Frequency"];
            int freq = Convert.ToInt32(value);
            freq = freq / 60000;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var time1 = loader.GetString("FrequencyInfo1");

            if(language == "pl-PL")
            {
                if (freq == 1)
                {
                    Time.Text = time1 + " " + freq.ToString() + " minutę";
                }
                else if (freq == 2 || freq == 3 || freq == 4)
                {
                    Time.Text = time1 + " " + freq.ToString() + " minuty";
                }
                else
                {
                    Time.Text = time1 + " " + freq.ToString() + " minut";
                }
            }
            else
            {
                if (freq == 1)
                {
                    Time.Text = time1 + " " + freq.ToString() + " minute";
                }
                else
                {
                    Time.Text = time1 + " " + freq.ToString() + " minutes";
                }

            }
        }


        private async void ChangeTime_Click(object sender, RoutedEventArgs e)
        {
            UnregisterNotifications();

            TimeOnly = true;

            AskTimeDialog TIME = new AskTimeDialog();
            await TIME.ShowAsync();

            GetTime();
        }

        public BindSettingsViewModel VIEWMODEL { get; set; }

        private async void Reset_Click(object sender, RoutedEventArgs e)
        {
            Switch.Toggled -= Switch_Toggled;
            ProgressRing ring = new ProgressRing();
            ring.IsActive = true;
            ResetStack.Children.Add(ring);
            RESETbutton.IsEnabled = false;

            #region Unregister
            UnregisterNotifications();

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("Frequncy");
            localSettings.Values.Remove("IsBackgroundEnabled");

            StartupTask startupTask = await StartupTask.GetAsync("RepeatsNotifi");
            startupTask.Disable();
            #endregion

            #region Reset IsEnabled in DB
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();
                String tableCommand = "UPDATE TitleTable SET IsEnabled='true'";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                //try
                //{
                createTable.ExecuteReader();
                //}
                //catch (SqliteException)
                //{

                //}
                db.Close();
            }

            this.VIEWMODEL = new BindSettingsViewModel();
            #endregion

            AskTimeDialog TIME = new AskTimeDialog();
            await TIME.ShowAsync();

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var reset = loader.GetString("ResetOK");

            GetTime();
            ResetStack.Children.Remove(ring);
            RESETbutton.Content = reset;
            CheckText.Visibility = Visibility.Visible;
            LISTofSets.ItemsSource = VIEWMODEL.SetRepeat;
            Switch.IsOn = true;
            Switch.Toggled += Switch_Toggled;
        }

        private void Set_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;

            if (toggle != null)
            {
                if(toggle.Tag != null)
                {
                    string name = toggle.Tag.ToString();

                    if (toggle.IsOn == true)
                    {
                        using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                        {
                            db.Open();
                            String tableCommand = "UPDATE TitleTable SET IsEnabled='true' WHERE TableName='" + name + "'";
                            SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                            try
                            {
                                createTable.ExecuteReader();
                            }
                            catch (SqliteException)
                            {

                            }
                            db.Close();
                        }
                    }
                    else
                    {
                        using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                        {
                            db.Open();
                            String tableCommand = "UPDATE TitleTable SET IsEnabled='false' WHERE TableName='" + name + "'";
                            SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                            try
                            {
                                createTable.ExecuteReader();
                            }
                            catch (SqliteException)
                            {

                            }
                            db.Close();
                        }

                        List<string> Enabled = GetFromDB.GrabData("TitleTable", "IsEnabled");
                        if (!Enabled.Contains("true"))
                        {
                            Switch.IsOn = false;
                        }
                    }
                }
            }
        }

        private async void Switch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    List<string> Enabled = GetFromDB.GrabData("TitleTable", "IsEnabled");
                    if (!Enabled.Contains("true"))
                    {
                        Switch.IsOn = false;
                    }
                    else
                    {
                        AskTimeDialog TIME = new AskTimeDialog();
                        await TIME.ShowAsync();

                        if (AskTimeDialog.IsCancel)
                        {
                            Switch.IsOn = false;
                        }
                        else
                        {
                            ChangeButton.IsEnabled = true;
                            LISTofSets.IsEnabled = true;
                        }
                        GetTime();
                    }
                }
                else
                {
                    if(language == "pl-PL")
                    {
                        Time.Text = "Powiadomienia przychodzą co 0 minut";
                    }
                    else
                    {
                        Time.Text = "Notifications are sent every 0 minutes";
                    }

                    ChangeButton.IsEnabled = false;
                    if(LISTofSets.Items.Count == 0)
                    {
                        LISTofSets.IsEnabled = false;
                    }
                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    localSettings.Values["Frequency"] = 0;
                    UnregisterNotifications();
                }
            }
        }

        async void UnregisterNotifications()
        {
            var exampleTaskName = "RepeatsNotificationTask";

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == exampleTaskName)
                {
                    task.Value.Unregister(true);
                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    localSettings.Values["IsBackgroundEnabled"] = false;
                    int id = Convert.ToInt32(localSettings.Values["ProcessId"]);
                    try
                    {
                        Process.GetProcessById(id).Kill();
                    }
                    catch
                    {

                    }
                    break;
                }
            }

            var tskName = "ToastBackgroundTask";
            foreach (var tsk in BackgroundTaskRegistration.AllTasks)
            {
                if (tsk.Value.Name == tskName)
                {
                    tsk.Value.Unregister(true);
                    break;
                }
            }


            StartupTask startupTask = await StartupTask.GetAsync("RepeatsNotifi");
            startupTask.Disable();
        }

        private async void RATE(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"ms-windows-store://review/?ProductId=9NXLCWT9DQF2");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);
        }

        private async void Feedback_Click(object sender, RoutedEventArgs e)
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }
    }
}
