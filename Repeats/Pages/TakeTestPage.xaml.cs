using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TakeTestPage : Page
    {
        Stopwatch stopwatch;
        public static string StopwatchResults;

        public TakeTestPage()
        {
            this.InitializeComponent();

            stopwatch = new Stopwatch();
            stopwatch.Start();

            this.ViewTestModel = new TakeTestPageDataModel();

            SetTitleBlock.Text = RepeatsList.OfficialName;
        }

        public TakeTestPageDataModel ViewTestModel { get; set; }

        public static List<string> yanswers;
        public static List<string> canswers;
        public static List<string> questions;
        public static List<string> emojis;
        public static int TestCount;

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Stop();

            StopwatchResults = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");

            var findalltext = GridTakeTest.FindDescendants<TextBox>();
            var gridviewtxt = findalltext.ToList();
            int count = gridviewtxt.Count;

            count--;

            yanswers = new List<string>();
            canswers = new List<string>();
            questions = new List<string>();
            emojis = new List<string>();

            TestCount = 0;

            for (int i = 0; i <= count; i++)
            {
                var element = ViewTestModel.Test.ElementAt(i);
                TextBox box = gridviewtxt[i];

                yanswers.Add(box.Text);
                canswers.Add(element.Answer);
                questions.Add(element.Question);

                if(element.Answer == yanswers.ElementAt(i))
                {
                    emojis.Add("✔");
                    TestCount++;
                }
                else
                {
                    emojis.Add("❌");
                }
            }

            Frame.Navigate(typeof(TestResults));
        }
    }
}
