using Repeats.Pages;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats
{
    public sealed partial class AskTimeDialog : ContentDialog
    {
        public static int time;
        public static bool IsCancel;

        public AskTimeDialog()
        {
            this.InitializeComponent();

            Load();
        }

        private void Load()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var time1 = loader.GetString("TIME1");
            var time2 = loader.GetString("TIME2");
            var time3 = loader.GetString("TIME3");

            TimeContent.Title = time1;
            TimeContent.PrimaryButtonText = time2;
            TimeContent.SecondaryButtonText = time3;

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

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            IsCancel = true;
            TimeContent.Hide();
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //try
            //{
            IsCancel = false;
                string txt = GetTimeTxt.Text;
                int GetIntTime = Int32.Parse(txt);
                GetIntTime = GetIntTime * 60000;

                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["Frequency"] = GetIntTime;
                localSettings.Values["IsBackgroundEnabled"] = true;



            var reResult = await BackgroundExecutionManager.RequestAccessAsync();

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

            if (taskRegistered == false)
            {
                var builder = new BackgroundTaskBuilder();

                ApplicationTrigger trigger = new ApplicationTrigger();

                builder.Name = exampleTaskName;
                builder.SetTrigger(trigger);
                builder.TaskEntryPoint = "BackgroundTask.Task";
                BackgroundTaskRegistration task = builder.Register();

                var result = await trigger.RequestAsync();
            }

            const string taskName = "ToastBackgroundTask";

            // If background task is already registered, do nothing
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(taskName)))
                return;

            // Otherwise request access
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            // Create the background task
            BackgroundTaskBuilder build = new BackgroundTaskBuilder()
            {
                Name = taskName
            };

            // Assign the toast action trigger
            build.SetTrigger(new ToastNotificationActionTrigger());

            // And register the task
            BackgroundTaskRegistration registration = build.Register();
            //}
            //catch (Exception)
            //{
            //    ExceptionUps();
            //}

            //StartupTask startupTask = await StartupTask.GetAsync("RepeatsNotifi");
            //startupTask.Disable();
            //switch (startupTask.State)
            //{
            //    case StartupTaskState.Disabled:
            //        // Task is disabled but can be enabled.
            //        StartupTaskState newState = await startupTask.RequestEnableAsync();
            //        Debug.WriteLine("Request to enable startup, result = {0}", newState);
            //        break;
            //    case StartupTaskState.DisabledByUser:
            //        // Task is disabled and user must enable it manually.
            //        MessageDialog dialog = new MessageDialog(
            //            "I know you don't want this app to run " +
            //            "as soon as you sign in, but if you change your mind, " +
            //            "you can enable this in the Startup tab in Task Manager.",
            //            "TestStartup");
            //        await dialog.ShowAsync();
            //        break;
            //    case StartupTaskState.DisabledByPolicy:
            //        Debug.WriteLine(
            //            "Startup disabled by group policy, or not supported on this device");
            //        break;
            //    case StartupTaskState.Enabled:
            //        Debug.WriteLine("Startup is enabled.");
            //        break;
            //}

        }
    }
}
