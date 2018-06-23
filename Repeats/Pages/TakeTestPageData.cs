using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeats.Pages
{
    public class TakeTestPageData
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class TakeTestPageDataModel
    {
        private ObservableCollection<TakeTestPageData> test = new ObservableCollection<TakeTestPageData>();
        public ObservableCollection<TakeTestPageData> Test { get { return this.test; } }
        public TakeTestPageDataModel()
        {
            List<String> Grab_Test()
            {
                List<String> questions = new List<String>();
                using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                {
                    db.Open();

                    SqliteCommand selectCommand = new SqliteCommand("SELECT question from " + RepeatsList.name, db);
                    SqliteDataReader query;
                    try
                    {
                        query = selectCommand.ExecuteReader();
                    }
                    catch (SqliteException error)
                    {
                        //Handle error
                        return questions;
                    }
                    while (query.Read())
                    {
                        questions.Add(query.GetString(0));
                    }
                    db.Close();
                }
                return questions;
            }

            List<String> Grab_correct()
            {
                List<String> answers = new List<String>();
                using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                {
                    db.Open();

                    SqliteCommand selectCommand = new SqliteCommand("SELECT answer from " + RepeatsList.name, db);
                    SqliteDataReader query;
                    try
                    {
                        query = selectCommand.ExecuteReader();
                    }
                    catch (SqliteException error)
                    {
                        //Handle error
                        return answers;
                    }
                    while (query.Read())
                    {
                        answers.Add(query.GetString(0));
                    }
                    db.Close();
                }
                return answers;
            }

            int count = Grab_Test().Count;

            for(int i = 0; i < count; i++)
            {
                this.test.Add(new TakeTestPageData() { Question = Grab_Test().ElementAt(i), Answer = Grab_correct().ElementAt(i) });
            }
        }
    }
}
