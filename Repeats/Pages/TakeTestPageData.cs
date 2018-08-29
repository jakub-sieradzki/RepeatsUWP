using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Repeats.Pages
{
    public class TakeTestPageData
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Image { get; set; }
        public Visibility visibility { get; set; }
    }

    public class TakeTestPageDataModel
    {
        private ObservableCollection<TakeTestPageData> test = new ObservableCollection<TakeTestPageData>();
        public ObservableCollection<TakeTestPageData> Test { get { return this.test; } }
        public TakeTestPageDataModel()
        {
            string NAME = RepeatsList.name;

            List<string> Grab_Test = GetFromDB.GrabData(NAME, "question");
            List<string> Grab_Correct = GetFromDB.GrabData(NAME, "answer");
            List<string> Grab_Images = GetFromDB.GrabData(NAME, "image");

            int count = Grab_Test.Count;

            for(int i = 0; i < count; i++)
            {
                if(Grab_Images.ElementAt(i) == "")
                {
                    this.test.Add(new TakeTestPageData() { Question = Grab_Test.ElementAt(i), Answer = Grab_Correct.ElementAt(i), Image = null, visibility = Visibility.Collapsed });
                }
                else
                {
                    this.test.Add(new TakeTestPageData() { Question = Grab_Test.ElementAt(i), Answer = Grab_Correct.ElementAt(i), Image = RepeatsList.folder.Path + "\\" + Grab_Images.ElementAt(i), visibility = Visibility.Visible });
                }


            }
        }
    }
}
