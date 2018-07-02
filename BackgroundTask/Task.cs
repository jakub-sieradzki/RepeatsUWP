using Microsoft.Data.Sqlite;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class Task: IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            object value = localSettings.Values["Frequency"];
            int freq = Convert.ToInt32(value);

            int c = 1;
            for (int i = 0; i < c; i++)
            {
                notifi();
                c++;
                Thread.Sleep(freq);
            }

        }

        void notifi()
        {
            IList<string> GetNames = GrabData("TitleTable", "TableName");
            IList<string> GetOfficial = GrabData("TitleTable", "title");

            int NameCount = GetNames.Count;

            Random rnd = new Random();
            int r = rnd.Next(NameCount);

            string name = GetNames[r];
            string ofname = GetOfficial[r];

            IList<string> qu = GrabData(name, "question");
            IList<string> an = GrabData(name, "answer");

            int ItemsCount = qu.Count;

            Random rnd2 = new Random();
            int r2 = rnd2.Next(ItemsCount);

            string question = qu[r2];
            string answer = an[r2];

            int conversationId = 384928;

            ToastVisual visual = new ToastVisual()
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

                    Attribution = new ToastGenericAttributionText()
                    {
                        Text = "Repeats (Beta)"
                    }
                }
            };

            ToastActionsCustom actions = new ToastActionsCustom()
            {
                Inputs =
                {
                    new ToastTextBox("tbReply")
                    {
                        PlaceholderContent = "Tutaj wpisz odpowiedź"
                    }
                },

                Buttons =
                {
                    new ToastButton("Reply", answer)
                    {
                        ActivationType = ToastActivationType.Background,
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
            toast.Tag = answer;

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
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException)
                {
                    return data;
                }
                while (query.Read())
                {
                    data.Add(query.GetString(0));
                }
                db.Close();
            }
            return data;
        }
    }
}
