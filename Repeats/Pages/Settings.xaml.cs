using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
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

        public Settings()
        {
            this.InitializeComponent();

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
                Time.Text = "Powiadomienia przychodzą co 0 minut";
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
        }

        void GetTime()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            object value = localSettings.Values["Frequency"];
            int freq = Convert.ToInt32(value);
            freq = freq / 60000;

            if (freq == 1)
            {
                Time.Text = "Powiadomienia wysyłane są co " + freq.ToString() + " minutę";
            }
            else
            {
                Time.Text = "Powiadomienia wysyłane są co " + freq.ToString() + " minut(y)";
            }
        }


        private async void ChangeTime_Click(object sender, RoutedEventArgs e)
        {
            UnregisterNotifications();

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

            GetTime();
            ResetStack.Children.Remove(ring);
            RESETbutton.Content = "Zresetowano pomyślnie";
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
                    Time.Text = "Powiadomienia przychodzą co 0 minut";
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

        void UnregisterNotifications()
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
        }
    }
}
