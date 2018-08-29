using Microsoft.Data.Sqlite;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class Task : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            object value = localSettings.Values["Frequency"];
            localSettings.Values["ProcessId"] = Process.GetCurrentProcess().Id;
            localSettings.Values["ThreadId"] = Thread.CurrentThread.ManagedThreadId;
            int freq = Convert.ToInt32(value);

            int c = 1;
            for (int i = 0; i < c; i++)
            {
                notifi();
                c++;
                Thread.Sleep(freq);
            }
            _deferral.Complete();
        }

        async void notifi()
        {
            IList<string> GetNames = GrabTitles("TitleTable", "TableName");
            IList<string> GetOfficial = GrabTitles("TitleTable", "title");
            IList<string> GetAvatars = GrabTitles("TitleTable", "Avatar");

            int NameCount = GetNames.Count;

            if (NameCount == 0)
            {
                var exampleTaskName = "RepeatsNotificationTask";

                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == exampleTaskName)
                    {
                        task.Value.Unregister(true);
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

                Process.GetCurrentProcess().Kill();
            }

            Random rnd = new Random();
            int r = rnd.Next(NameCount);

            string name = GetNames[r];
            string ofname = GetOfficial[r];
            string avatar = GetAvatars[r];

            IList<string> qu = GrabData(name, "question");
            IList<string> an = GrabData(name, "answer");
            IList<string> im = GrabData(name, "image");

            int ItemsCount = qu.Count;

            Random rnd2 = new Random();
            int r2 = rnd2.Next(ItemsCount);

            string question = qu[r2];
            string answer = an[r2];
            string image = im[r2];

            int conversationId = 384928;

            ToastVisual visual;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var answerhere = loader.GetString("AnswerHere");

            if (image != "")
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Images");
                StorageFile img = await folder.GetFileAsync(image);
                string path = img.Path;


                visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = ofname
                            },

                            new AdaptiveText()
                            {
                                Text = question
                            },
                        },
                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = path
                        },

                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = avatar,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                    }
                };
            }
            else
            {
                visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = ofname
                            },

                            new AdaptiveText()
                            {
                                Text = question
                            },
                        },

                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = avatar,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                    }
                };
            }

            ToastActionsCustom actions = new ToastActionsCustom()
            {
                Inputs =
                {
                    new ToastTextBox("tbReply")
                    {
                        PlaceholderContent = answerhere
                    }
                },

                Buttons =
                {
                    new ToastButton("Reply", answer)
                    {
                        ActivationType = ToastActivationType.Background,
                        ImageUri = "ms-appx:///Assets/checking.png",
                        TextBoxId = "tbReply"
                    }
                }
            };

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions,

                Launch = new QueryString()
                {
                    {"action", "viewQuestion" },
                    {"conversationId", conversationId.ToString() }
                }.ToString()
            };

            var toast = new ToastNotification(toastContent.GetXml());
            toast.Tag = "NextQuestion";

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static IList<string> GrabData(string FROM, string WHAT)
        {
            IList<string> data = new List<string>();
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT " + WHAT + " from " + FROM, db);
                SqliteDataReader query;
                //try
                //{
                query = selectCommand.ExecuteReader();
                //}
                //catch (SqliteException)
                //{
                //return data;
                //}
                while (query.Read())
                {
                    data.Add(query.GetString(0));
                }
                db.Close();
            }
            return data;
        }

        public static IList<string> GrabTitles(string FROM, string WHAT)
        {
            IList<string> titles = new List<string>();

            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT " + WHAT + " from " + FROM + " WHERE " + "IsEnabled='true'", db);
                SqliteDataReader query;
                //try
                //{
                query = selectCommand.ExecuteReader();
                //}
                //catch (SqliteException)
                //{
                //return data;
                //}
                while (query.Read())
                {
                    titles.Add(query.GetString(0));
                }
                db.Close();
            }
            return titles;
        }
    }
}
