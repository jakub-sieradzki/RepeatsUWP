using Repeats.Pages;
using System;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats
{
    public sealed partial class AskTimeDialog : ContentDialog
    {
        public static int time;

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
            TimeContent.Hide();
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //try
            //{

                string txt = GetTimeTxt.Text;
                int GetIntTime = Int32.Parse(txt);
                GetIntTime = GetIntTime * 60000;

                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                localSettings.Values["Frequency"] = GetIntTime;

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
                    builder.TaskEntryPoint = "BackgroundTask.Task";
                    builder.SetTrigger(trigger);
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

        }
    }
}
