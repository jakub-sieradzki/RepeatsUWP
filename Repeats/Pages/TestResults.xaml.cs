using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

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
            int good = TakeTestPage.TestCount;
            int all = ViewResultsModel.Test.Count;
            int wrong = all - good;

            Score1.Text = "Poprawnych odpowiedzi: " + good.ToString();
            Score2.Text = "Niepoprawnych odpowiedzi: " + wrong.ToString();
            Score3.Text = "Wszystkich odpowiedzi: " + all.ToString();
            Score4.Text = "Twój czas: " + TakeTestPage.StopwatchResults;
            
            var value = ((double)good / all) * 100;
            var percentage = Convert.ToInt32(Math.Round(value, 0));

            percentScore.Text = percentage.ToString() + "%";
            Radial.Value = percentage;
        }
    }
}
