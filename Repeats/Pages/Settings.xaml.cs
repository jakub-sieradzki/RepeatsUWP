using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
                Switch.IsOn = false;
            }

            Switch.Toggled += Switch_Toggled;

            this.VIEWMODEL = new BindSettingsViewModel();

            int i = 0;
        }

        public BindSettingsViewModel VIEWMODEL { get; set; }

        private async void Reset_Click(object sender, RoutedEventArgs e)
        {
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
            LISTofSets.ItemsSource = VIEWMODEL.SetRepeat;
            #endregion

            AskTimeDialog TIME = new AskTimeDialog();
            await TIME.ShowAsync();

            ResetStack.Children.Remove(ring);
            RESETbutton.Content = "Zresetowano pomyślnie";
            CheckText.Visibility = Visibility.Visible;
        }

        private void Set_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;


            if (toggle != null)
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
                    AskTimeDialog TIME = new AskTimeDialog();
                    await TIME.ShowAsync();

                    if(AskTimeDialog.IsCancel)
                    {
                        Switch.IsOn = false;
                    }
                }
                else
                {
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

            BackgroundExecutionManager.RemoveAccess();
        }
    }

}
