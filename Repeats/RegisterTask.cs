using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Repeats
{
    class RegisterTask
    {
        public static async void RegisterBackgroundTask()
        {
            try
            {
                StorageFolder MainStorage = ApplicationData.Current.LocalFolder;
                StorageFolder Folders = await MainStorage.GetFolderAsync("FOLDERS");

                IReadOnlyList<StorageFolder> groupedItems = await Folders.GetFoldersAsync();

                StorageFile samplefile3 = await MainStorage.GetFileAsync("time.txt");
                string t = await FileIO.ReadTextAsync(samplefile3);

                string time;

                using (StringReader reader = new StringReader(t))
                {
                    time = reader.ReadLine();
                }

                int b = Int32.Parse(time);

                uint u = Convert.ToUInt32(b);

                var exampleTaskName = "RepeatsNotificationTask";

                foreach (var task1 in BackgroundTaskRegistration.AllTasks)
                {
                    if (task1.Value.Name == exampleTaskName)
                    {
                        break;
                    }
                }

                var builder = new BackgroundTaskBuilder();

                builder.Name = exampleTaskName;
                builder.TaskEntryPoint = "NotificationsBackground.Background";
                builder.SetTrigger(new TimeTrigger(u, false));

                BackgroundTaskRegistration task = builder.Register();
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

    }
}
