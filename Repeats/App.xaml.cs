using NotificationsBackground;
using Repeats.Pages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite.Internal;

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

            SqliteEngine.UseWinSqlite3(); //Configuring library to use SDK version of SQLite
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();
                String tableCommand = "CREATE TABLE IF NOT EXISTS TitleTable (id INTEGER PRIMARY KEY AUTOINCREMENT, title NVARCHAR(2048) NULL, TableName NVARCHAR(2048) NULL, CreateDate NVARCHAR(2048) NULL)";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                try
                {
                    createTable.ExecuteReader();
                }
                catch (SqliteException)
                {

                }
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            await OnLaunchedOrActivated(e);

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    await statusBar.HideAsync();
                }
            }

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

                    var test = await storageFolder.TryGetItemAsync("OOBE");
                    if (test == null)
                    {
                        rootFrame.Navigate(typeof(WelcomePage));
                        StorageFile storageFile = await storageFolder.CreateFileAsync("OOBE");
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(MainPage), e.Arguments);
                    }
                }

                Window.Current.Activate();
            }
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            await OnLaunchedOrActivated(e);
        }

        private async Task OnLaunchedOrActivated(IActivatedEventArgs e)
        {
            await InitializeApp();

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
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            // Handle toast activation
            if (e is ToastNotificationActivatedEventArgs)
            {
                var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

                // If empty args, no specific action (just launch the app)
                if (toastActivationArgs.Argument.Length == 0)
                {
                    if (rootFrame.Content == null)
                        rootFrame.Navigate(typeof(MainPage));
                }
            }

            // Handle launch activation
            else if (e is LaunchActivatedEventArgs)
            {
                var launchActivationArgs = e as LaunchActivatedEventArgs;

                // If launched with arguments (not a normal primary tile/applist launch)
                if (launchActivationArgs.Arguments.Length > 0)
                {
                    // TODO: Handle arguments for cases like launching from secondary Tile, so we navigate to the correct page
                    throw new NotImplementedException();
                }

                // Otherwise if launched normally
                else
                {
                    // If we're currently not on a page, navigate to the main page
                    if (rootFrame.Content == null)
                    {
                        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

                        var test = await storageFolder.TryGetItemAsync("OOBE");
                        if (test == null)
                        {
                            rootFrame.Navigate(typeof(WelcomePage));
                            StorageFile storageFile = await storageFolder.CreateFileAsync("OOBE");
                        }
                        else
                        {
                            rootFrame.Navigate(typeof(MainPage));
                        }
                    }
                }
            }

            else
            {
                // TODO: Handle other types of activation
                throw new NotImplementedException();
            }


            // Ensure the current window is active
            Window.Current.Activate();


        }

        private bool _isInitialized = false;
        private async Task InitializeApp()
        {
            if (_isInitialized)
                return;

            RegisterBackgroundTask();

            _isInitialized = true;
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

        private void RegisterBackgroundTask()
        {
            const string taskName = "ToastBackgroundTask";

            // If background task is already registered, do nothing
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(taskName)))
                return;

            // Otherwise create the background task
            var builder = new BackgroundTaskBuilder()
            {
                Name = taskName,
                TaskEntryPoint = typeof(TextBackground).FullName
            };

            // And set the toast action trigger
            builder.SetTrigger(new ToastNotificationActionTrigger());

            // And register the task

            builder.Register();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}

