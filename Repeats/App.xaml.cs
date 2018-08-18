﻿using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite.Internal;
using Windows.UI.Notifications;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;
using Windows.Storage;
using System.Linq;
using Microsoft.Toolkit.Uwp.Helpers;
using System.IO;

namespace Repeats
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            //this.Suspending += OnSuspending;

            SqliteEngine.UseWinSqlite3();
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();
                String tableCommand = "CREATE TABLE IF NOT EXISTS TitleTable (id INTEGER PRIMARY KEY AUTOINCREMENT, title NVARCHAR(2048) NULL, TableName NVARCHAR(2048) NULL, CreateDate NVARCHAR(2048) NULL, IsEnabled NVARCHAR(2048) NULL)";
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

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            string ApplicationVersion = $"{SystemInformation.ApplicationVersion.Major}.{SystemInformation.ApplicationVersion.Minor}.{SystemInformation.ApplicationVersion.Build}.{SystemInformation.ApplicationVersion.Revision}";
            bool IsAppUpdated = SystemInformation.IsAppUpdated;

            if (ApplicationVersion == "2.0.0.0" && IsAppUpdated)
            {
                StorageFolder c = await ApplicationData.Current.LocalFolder.GetFolderAsync("FOLDERS");
                var folders = await c.GetFoldersAsync();
                int countoffolders = folders.Count;

                for (int i = 0; i < countoffolders; i++)
                {
                    var getFolder = folders.ElementAt(i);
                    string name = getFolder.DisplayName;

                    StorageFolder sampleFile3 = await c.GetFolderAsync(name);

                    StorageFile sampleFile2 = await sampleFile3.GetFileAsync("ItemsCount.txt");

                    string w = await FileIO.ReadTextAsync(sampleFile2);

                    string count1;

                    using (StringReader reader = new StringReader(w))
                    {
                        count1 = reader.ReadLine();
                    }

                    int d = Int32.Parse(count1);

                    int a = i;

                    a++;

                    DateTime create = sampleFile3.DateCreated.DateTime;
                    string realDate = create.ToShortDateString();

                    DateTime getTime = DateTime.Now;
                    getTime = getTime.AddSeconds(a);
                    string date = getTime.ToString("yyyyMMddHHmmss");
                    date = "R" + date;

                    using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                    {
                        db.Open();

                        #region Create table
                        String tableCommand = "CREATE TABLE IF NOT EXISTS " + date + " (id INTEGER PRIMARY KEY AUTOINCREMENT, question NVARCHAR(2048) NULL, answer NVARCHAR(2048) NULL, image NVARCHAR(2048) NULL)";
                        SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                        //try
                        //{
                        createTable.ExecuteReader();
                        //}
                        //catch(SqliteException)
                        //{

                        //}
                        #endregion

                        #region Get & save sets
                        for (int j = 1; j <= d; j++)
                        {
                            StorageFile sampleFile5 = await sampleFile3.GetFileAsync("header" + j.ToString() + ".txt");

                            string q = await FileIO.ReadTextAsync(sampleFile5);

                            string line1;
                            string line2;
                            string line3;

                            using (StringReader reader = new StringReader(q))
                            {
                                line1 = reader.ReadLine();
                                line2 = reader.ReadLine();
                                line3 = reader.ReadLine();
                            }

                            SqliteCommand insertCommand = new SqliteCommand();
                            insertCommand.Connection = db;

                            insertCommand.CommandText = "INSERT INTO " + date + " VALUES (NULL, @question, @answer, @image);";
                            insertCommand.Parameters.AddWithValue("@question", line2);
                            insertCommand.Parameters.AddWithValue("@answer", line3);
                            insertCommand.Parameters.AddWithValue("@image", "");
                            //try
                            //{
                            insertCommand.ExecuteReader();
                            //}
                            //catch (SqliteException)
                            //{
                            //    return;
                            //}
                        }
                        #endregion

                        #region save to RepeatsList
                        SqliteCommand insertCommand2 = new SqliteCommand();
                        insertCommand2.Connection = db;

                        insertCommand2.CommandText = "INSERT INTO TitleTable VALUES (NULL, @title, @TableName, @CreateDate, @IsEnabled);";
                        insertCommand2.Parameters.AddWithValue("@title", name);
                        insertCommand2.Parameters.AddWithValue("@TableName", date);
                        insertCommand2.Parameters.AddWithValue("@CreateDate", realDate);
                        insertCommand2.Parameters.AddWithValue("@IsEnabled", "true");
                        try
                        {
                            insertCommand2.ExecuteReader();
                        }
                        catch (SqliteException)
                        {
                            return;
                        }
                        #endregion

                    }
                }
                await c.DeleteAsync();
            }
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        public bool IsStartup;

        protected override void OnActivated(IActivatedEventArgs e)
        {
            if (e is ToastNotificationActivatedEventArgs)
            {
                IsStartup = false;

                Frame rootFrame = Window.Current.Content as Frame;

                var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

                QueryString args = QueryString.Parse(toastActivationArgs.Argument);

                rootFrame.Navigate(typeof(MainPage));

                if (rootFrame.BackStack.Count == 0)
                    rootFrame.BackStack.Add(new PageStackEntry(typeof(MainPage), null, null));

                Window.Current.Activate();
            }
            else if (e.Kind == ActivationKind.StartupTask)
            {
                IsStartup = true;
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var deferral = args.TaskInstance.GetDeferral();

            switch (args.TaskInstance.Task.Name)
            {
                case "ToastBackgroundTask":
                    var details = args.TaskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
                    if (details != null)
                    {
                        string arguments = details.Argument;
                        var userInput = details.UserInput;
                        var input = userInput.Values;

                        if (arguments == "next")
                        {
                            notifi();
                        }
                        else if(arguments == "cancel")
                        {

                        }
                        else
                        {
                            if (input.Contains(arguments))
                            {
                                int conversationId = 00111;
                                ToastVisual visual = new ToastVisual()
                                {
                                    BindingGeneric = new ToastBindingGeneric()
                                    {
                                        Children =
                                    {
                                        new AdaptiveText()
                                        {
                                            Text = "Brawo! To poprawna odpowiedź"
                                        },

                                        new AdaptiveText()
                                        {
                                            Text = "Szybko się uczysz 😉"
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

                                    Buttons =
                                    {
                                        new ToastButton("Odrzuć", "cancel")
                                        {
                                            ActivationType = ToastActivationType.Background,
                                        },

                                        new ToastButton("Kolejne pytanie", "next")
                                        {
                                            ActivationType = ToastActivationType.Background,

                                            ActivationOptions = new ToastActivationOptions()
                                            {
                                                AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate
                                            }
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

                                var toast = new ToastNotification(toastContent.GetXml())
                                {
                                    Tag = "NextQuestion"
                                };

                                ToastNotificationManager.CreateToastNotifier().Show(toast);
                            }
                            else
                            {
                                int conversationId = 00111;
                                ToastVisual visual = new ToastVisual()
                                {
                                    BindingGeneric = new ToastBindingGeneric()
                                    {
                                        Children =
                                    {
                                        new AdaptiveText()
                                        {
                                            Text = "Niestety, to nie była poprawna odpowiedź"
                                        },

                                        new AdaptiveText()
                                        {
                                            Text = "Prawidłowa odpowiedź to: " + arguments
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

                                    Buttons =
                                    {
                                        new ToastButton("Odrzuć", "cancel")
                                        {
                                            ActivationType = ToastActivationType.Background,
                                        },

                                        new ToastButton("Kolejne pytanie", "next")
                                        {
                                            ActivationType = ToastActivationType.Background,
                                            ActivationOptions = new ToastActivationOptions()
                                            {
                                                AfterActivationBehavior = ToastAfterActivationBehavior.PendingUpdate
                                            }
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

                                var toast = new ToastNotification(toastContent.GetXml())
                                {
                                    Tag = "NextQuestion"
                                };

                                ToastNotificationManager.CreateToastNotifier().Show(toast);
                            }
                        }
                    }
                    break;

                //case "RepeatsNotificationTask":
                //    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                //    object value = localSettings.Values["Frequency"];
                //    int freq = Convert.ToInt32(value);

                //    int c = 1;
                //    //for (int i = 0; i < c; i++)
                //    //{
                //    //    notifi();
                //    //    c++;
                //    //    Thread.Sleep(freq);
                //    //}

                //    System.Timers.Timer timer = new System.Timers.Timer(10000);
                //    timer.Elapsed += nNotifi;
                //    timer.AutoReset = true;
                //    timer.Enabled = true;
                //    break;
            }

            deferral.Complete();
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

            var toast = new ToastNotification(toastContent.GetXml())
            {
                Tag="NextQuestion"
            };
            

            ToastNotificationManager.CreateToastNotifier().Show(toast);

            int i = 0;
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

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }


        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>

        //private async void OnSuspending(object sender, SuspendingEventArgs e)
        //{
        //    var deferral = e.SuspendingOperation.GetDeferral();
        //   if(IsStartup)
        //   {
        //        var reResult = await BackgroundExecutionManager.RequestAccessAsync();

        //        var exampleTaskName = "RepeatsNotificationTask";

        //        foreach (var task in BackgroundTaskRegistration.AllTasks)
        //        {
        //            if (task.Value.Name == exampleTaskName)
        //            {
        //                task.Value.Unregister(true);
        //                break;
        //            }
        //        }

        //        var builder = new BackgroundTaskBuilder();

        //        ApplicationTrigger trigger = new ApplicationTrigger();

        //        builder.Name = exampleTaskName;
        //        builder.SetTrigger(trigger);
        //        builder.TaskEntryPoint = "BackgroundTask.Task";
        //        BackgroundTaskRegistration builders = builder.Register();

        //        var result = await trigger.RequestAsync();

        //        const string taskName = "ToastBackgroundTask";

        //        foreach (var tasks in BackgroundTaskRegistration.AllTasks)
        //        {
        //            if (tasks.Value.Name == taskName)
        //            {
        //                tasks.Value.Unregister(true);
        //                break;
        //            }
        //        }

        //        BackgroundTaskBuilder build = new BackgroundTaskBuilder()
        //        {
        //            Name = taskName
        //        };

        //        build.SetTrigger(new ToastNotificationActionTrigger());

        //        BackgroundTaskRegistration registration = build.Register();

        //        Process.GetCurrentProcess().Kill();
        //    }
        //    deferral.Complete();
        //}
    }
}

