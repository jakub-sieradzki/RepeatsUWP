using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace NotificationsBackground
{
    public sealed class TextBackground : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            if (taskInstance.TriggerDetails is ToastNotificationActionTriggerDetail)
            {
                var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;

                QueryString args = QueryString.Parse(details.Argument);

                switch (args["action"])
                {
                    case "reply":
                        await HandleReply(details, args);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            else
                throw new NotImplementedException();

            deferral.Complete();

        }

        private async Task HandleReply(ToastNotificationActionTriggerDetail details, QueryString args)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var notification3 = loader.GetString("notifi3");
            var notification4 = loader.GetString("notifi4");
            var notification5 = loader.GetString("notifi5");

            StorageFolder storageFolder1 = ApplicationData.Current.LocalFolder;

            StorageFile samplefile2 = await storageFolder1.GetFileAsync("question.txt");

            string q = await FileIO.ReadTextAsync(samplefile2);

            string line1;

            using (StringReader reader = new StringReader(q))
            {
                line1 = reader.ReadLine();
            }

            int conversationId = int.Parse(args["conversationId"]);

            string message = (string)details.UserInput["tbReply"];

            await Task.Delay(TimeSpan.FromSeconds(2.3));

            if (message == line1)
            {
                var toastContent = new ToastContent()
                {
                    Visual = new ToastVisual()
                    {
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "Repeats"
                                },
                                new AdaptiveText()
                                {
                                    Text = notification3
                                }
                            }
                        }
                    }
                };

                var toastNotif = new ToastNotification(toastContent.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
            }
            else
            {
                var toastContent = new ToastContent()
                {
                    Visual = new ToastVisual()
                    {
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = notification4
                                },
                                new AdaptiveText()
                                {
                                    Text = notification5 + line1
                                }
                            }
                        }
                    }
                };

                var toastNotif = new ToastNotification(toastContent.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
            }
        }
    }
}
