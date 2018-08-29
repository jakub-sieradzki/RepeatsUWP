using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
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

            IsSecondaryButtonEnabled = false;
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

        private void GetTime_Changed(object sender, RoutedEventArgs e)
        {
            int time = GetTimeTxt.Text.Length;
            Int32.TryParse(GetTimeTxt.Text, out int number);
            if (time > 0)
            {
                IsSecondaryButtonEnabled = true;
            }
            if(time == 0 || number == 0)
            {
                IsSecondaryButtonEnabled = false;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            IsCancel = true;
            TimeContent.Hide();
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if(Pages.Settings.TimeOnly == true)
            {
                string txt = GetTimeTxt.Text;
                int GetIntTime = Int32.Parse(txt);
                GetIntTime = GetIntTime * 60000;
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["Frequency"] = GetIntTime;
            }
            else
            {
                IsCancel = false;
                string txt = GetTimeTxt.Text;
                int GetIntTime = Int32.Parse(txt);
                GetIntTime = GetIntTime * 60000;

                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["Frequency"] = GetIntTime;
                localSettings.Values["IsBackgroundEnabled"] = true;

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

                if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(taskName)))
                    return;

                BackgroundTaskBuilder build = new BackgroundTaskBuilder()
                {
                    Name = taskName
                };

                build.SetTrigger(new ToastNotificationActionTrigger());
                BackgroundTaskRegistration registration = build.Register();

                StartupTask startupTask = await StartupTask.GetAsync("RepeatsNotifi");

                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var dialog1 = loader.GetString("DIALOG1");
                var dialog2 = loader.GetString("DIALOG2");

                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                        // Task is disabled but can be enabled.
                        StartupTaskState newState = await startupTask.RequestEnableAsync();
                        Debug.WriteLine("Request to enable startup, result = {0}", newState);
                        if (newState == StartupTaskState.DisabledByUser)
                        {

                            ContentDialog warning1 = new ContentDialog
                            {
                                Title = dialog1,
                                Content = dialog2,
                                PrimaryButtonText = "OK"
                            };
                            await warning1.ShowAsync();
                        }
                        break;
                    case StartupTaskState.DisabledByUser:
                        // Task is disabled and user must enable it manually.
                        ContentDialog warning = new ContentDialog
                        {
                            Title = dialog1,
                            Content = dialog2,
                            PrimaryButtonText = "OK"
                        };
                        await warning.ShowAsync();
                        break;
                    case StartupTaskState.DisabledByPolicy:
                        Debug.WriteLine(
                            "Startup disabled by group policy, or not supported on this device");
                        break;
                    case StartupTaskState.Enabled:
                        Debug.WriteLine("Startup is enabled.");
                        break;
                }
            }
        }
    }
}
