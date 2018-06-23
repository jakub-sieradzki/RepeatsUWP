using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Repeats.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public class testBind
    {
        public string AllQuestions { get; set; }
        public string CorrectAnswers { get; set; }
        public string YourAnswers { get; set; }
        public string emoji { get; set; }
    }

    public class testBindModel
    {
        private ObservableCollection<testBind> test = new ObservableCollection<testBind>();
        public ObservableCollection<testBind> Test { get { return this.test; } } 
        public testBindModel()
        {
            int count = TakeTestPage.yanswers.Count;
            count--;

            for (int i = 0; i <= count; i++)
            {
                Test.Add(new testBind() { AllQuestions = TakeTestPage.questions.ElementAt(i), CorrectAnswers = "Poprawna odpowiedź: " + TakeTestPage.canswers.ElementAt(i), YourAnswers = "Twoja odpowiedź: " + TakeTestPage.yanswers.ElementAt(i), emoji = TakeTestPage.emojis.ElementAt(i) });
            }
        }
    }


    public sealed partial class TestResults : Page
    {
        public TestResults()
        {
            this.InitializeComponent();

            this.ViewResultsModel = new testBindModel();
            Load();
        }

        public testBindModel ViewResultsModel { get; set; }

        private void Load()
        {
            Score.Text = TakeTestPage.TestCount.ToString() + "/" + ViewResultsModel.Test.Count + " poprawnych odpowiedzi";

            var value = ((double)TakeTestPage.TestCount / ViewResultsModel.Test.Count) * 100;
            var percentage = Convert.ToInt32(Math.Round(value, 0));

            percentScore.Text = percentage.ToString() + "%";
            Radial.Value = percentage;
        }
    }
}
