using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace NotificationsBackground
{
    public sealed class Background : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var notification1 = loader.GetString("notifi1");
            var notification2 = loader.GetString("notifi2");

            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            int conversationId = 384928;

            StorageFolder storageFolder1 = ApplicationData.Current.LocalFolder;

            StorageFolder sampleFile1 = await storageFolder1.GetFolderAsync("FOLDERS");

            IReadOnlyList<StorageFolder> groupedItems = await sampleFile1.GetFoldersAsync();

            Random rnd = new Random();

            int r = rnd.Next(groupedItems.Count);

            var b = groupedItems[r];

            string NAME = b.DisplayName;

            StorageFolder sampleFile3 = await sampleFile1.GetFolderAsync(NAME);

            //----------------------------------------------------------------------//

            StorageFile sampleFile2 = await sampleFile3.GetFileAsync("ItemsCount.txt");

            string w = await FileIO.ReadTextAsync(sampleFile2);

            string count1;

            using (StringReader reader = new StringReader(w))
            {
                count1 = reader.ReadLine();
            }

            int c = Int32.Parse(count1);

            c += 1;

            //Get Items count

            Random rnd1 = new Random();

            int a = rnd1.Next(1, c);

            StorageFile sampleFile5 = await sampleFile3.GetFileAsync("header" + a.ToString() + ".txt");

            string q = await FileIO.ReadTextAsync(sampleFile5);

            // Random item to notifi

            string line1;
            string line2;
            string line3;

            using (StringReader reader = new StringReader(q))
            {
                line1 = reader.ReadLine();
                line2 = reader.ReadLine();
                line3 = reader.ReadLine();
            }

            StorageFile sampleFile = await storageFolder1.CreateFileAsync("question.txt", CreationCollisionOption.ReplaceExisting);

            StorageFile sampleFile6 = await storageFolder1.GetFileAsync("question.txt");

            await FileIO.WriteTextAsync(sampleFile6, line3);

            //Read lines for notifi

            // NOTIFICATION AREA

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
                                Text = line1,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = line2
                            }
                        },
                        Attribution = new ToastGenericAttributionText()
                        {
                            Text = "Repeats"
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("tbReply")
                        {
                            PlaceholderContent = notification1
                        }
                    },
                    Buttons =
                    {
                        new ToastButton(notification2, new QueryString()
                        {
                            {"action", "reply" },
                            {"conversationId", conversationId.ToString() }
                        }.ToString())
                        {
                            ActivationType = ToastActivationType.Background,
                            TextBoxId = "tbReply"
                        },
                    }
                },
                Launch = "conversationId"
            };

            // END NOTIFICATION AREA

            var toast = new ToastNotification(toastContent.GetXml());

            toast.ExpirationTime = DateTime.Now.AddMinutes(10);

            ToastNotificationManager.CreateToastNotifier().Show(toast);

            deferral.Complete();
        }
    }
}
