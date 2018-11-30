using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;
using Windows.Storage;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel;
using System.Diagnostics;
using DataAccessLibrary;

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
            this.Suspending += OnSuspending;

            DataAccess.CreateDatabase();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            string ApplicationVersion = $"{SystemInformation.ApplicationVersion.Major}.{SystemInformation.ApplicationVersion.Minor}.{SystemInformation.ApplicationVersion.Build}.{SystemInformation.ApplicationVersion.Revision}";
            bool IsAppUpdated = SystemInformation.IsAppUpdated;

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
            if (e.Kind == ActivationKind.StartupTask)
            {
                IsStartup = true;
            }
            else
            {
                IsStartup = false;

                Frame rootFrame = Window.Current.Content as Frame;

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

                var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

                QueryString args = QueryString.Parse(toastActivationArgs.Argument);

                rootFrame.Navigate(typeof(MainPage));

                if (rootFrame.BackStack.Count == 0)
                    rootFrame.BackStack.Add(new PageStackEntry(typeof(MainPage), null, null));

                Window.Current.Activate();
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var deferral = args.TaskInstance.GetDeferral();

            var loader2 = new Windows.ApplicationModel.Resources.ResourceLoader();
            var respond1 = loader2.GetString("NotifiCorrect");
            var respond2 = loader2.GetString("Notifigreat");
            var respond3 = loader2.GetString("Cancel");
            var respond4 = loader2.GetString("NextQuestion");
            var respond5 = loader2.GetString("NotifiIncorrect");
            var respond6 = loader2.GetString("ShowCorrect");

            switch (args.TaskInstance.Task.Name)
            {
                case "ToastBackgroundTask":
                    var details = args.TaskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
                    if (details != null)
                    {
                        var arguments = details.Argument;
                        var userInput = details.UserInput;
                        var input = userInput.Values;

                        if (arguments == "next")
                        {
                            notifi();
                        }
                        else if(arguments == "cancel")
                        {

                        }
                        else if(arguments == null)
                        {
                            Frame rootFrame = Window.Current.Content as Frame;

                            rootFrame.Navigate(typeof(MainPage));

                            if (rootFrame.BackStack.Count == 0)
                                rootFrame.BackStack.Add(new PageStackEntry(typeof(MainPage), null, null));

                            Window.Current.Activate();
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
                                                Text = respond1
                                            },

                                            new AdaptiveText()
                                            {
                                                Text = respond2 + " " + "😉"
                                            },
                                        }
                                    }
                                };

                                ToastActionsCustom actions = new ToastActionsCustom()
                                {

                                    Buttons =
                                    {
                                        new ToastButton(respond3, "cancel")
                                        {
                                            ActivationType = ToastActivationType.Background,
                                        },

                                        new ToastButton(respond4, "next")
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
                                            Text = respond5
                                        },

                                        new AdaptiveText()
                                        {
                                            Text = respond6 + " " + arguments
                                        },
                                    },
                                    }
                                };

                                ToastActionsCustom actions = new ToastActionsCustom()
                                {

                                    Buttons =
                                    {
                                        new ToastButton(respond3, "cancel")
                                        {
                                            ActivationType = ToastActivationType.Background,
                                        },

                                        new ToastButton(respond4, "next")
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
            }

            deferral.Complete();
        }

        async void notifi()
        {
            IList<string> GetNames = DataAccess.GrabTitles("TitleTable", "TableName");
            IList<string> GetOfficial = DataAccess.GrabTitles("TitleTable", "title");
            IList<string> GetAvatars = DataAccess.GrabTitles("TitleTable", "Avatar");

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var answerhere = loader.GetString("AnswerHere");

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

            IList<string> qu = DataAccess.GrabData(name, "question");
            IList<string> an = DataAccess.GrabData(name, "answer");
            IList<string> im = DataAccess.GrabData(name, "image");

            int ItemsCount = qu.Count;

            Random rnd2 = new Random();
            int r2 = rnd2.Next(ItemsCount);

            string question = qu[r2];
            string answer = an[r2];
            string image = im[r2];

            int conversationId = 384928;

            ToastVisual visual;

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
                        }
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
                        }
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

            var toast = new ToastNotification(toastContent.GetXml())
            {
                Tag = "NextQuestion"
            };

            ToastNotificationManager.CreateToastNotifier().Show(toast);
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

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            if (IsStartup)
            {
                var reResult = await BackgroundExecutionManager.RequestAccessAsync();

                var exampleTaskName = "RepeatsNotificationTask";

                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == exampleTaskName)
                    {
                        task.Value.Unregister(true);
                        break;
                    }
                }

                var builder = new BackgroundTaskBuilder();

                ApplicationTrigger trigger = new ApplicationTrigger();

                builder.Name = exampleTaskName;
                builder.SetTrigger(trigger);
                builder.TaskEntryPoint = "BackgroundTask.Task";
                BackgroundTaskRegistration builders = builder.Register();

                var result = await trigger.RequestAsync();

                const string taskName = "ToastBackgroundTask";

                foreach (var tasks in BackgroundTaskRegistration.AllTasks)
                {
                    if (tasks.Value.Name == taskName)
                    {
                        tasks.Value.Unregister(true);
                        break;
                    }
                }

                BackgroundTaskBuilder build = new BackgroundTaskBuilder()
                {
                    Name = taskName
                };

                build.SetTrigger(new ToastNotificationActionTrigger());

                BackgroundTaskRegistration registration = build.Register();

                Process.GetCurrentProcess().Kill();
            }
            deferral.Complete();
        }
    }
}

