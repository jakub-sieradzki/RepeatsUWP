using System;
using System.Diagnostics;
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

            if(taskRegistered)
            {
                Switch.IsOn = true;
            }
            else
            {
                Switch.IsOn = false;
            }

            Switch.Toggled += Switch_Toggled;
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
                }
                else
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
                            Process.GetProcessById(id).Kill();
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
    }
}
