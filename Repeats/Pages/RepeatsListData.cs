using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Repeats.Pages
{
    public class RepeatsListData
    {
        public string ProjectName { get; set; }
        public string ProjectDate { get; set; }
    }

    public class MainPageDataModel
    {
        private ObservableCollection<RepeatsListData> datas = new ObservableCollection<RepeatsListData>();
        public ObservableCollection<RepeatsListData> Datas { get { return this.datas; } }
        public MainPageDataModel()
        {
            int count;
            //using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            //{
            //    db.Open();
            //    SqliteCommand selectCommand = new SqliteCommand("SELECT title from TitleTable", db);
            //    count = selectCommand.Parameters.Count;
            //    db.Close();
            //}

            List<String> Grab_Titles()
            {
                List<String> title = new List<string>();
                using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                {
                    db.Open();

                    SqliteCommand selectCommand = new SqliteCommand("SELECT title from TitleTable", db);
                    SqliteDataReader query;
                    try
                    {
                        query = selectCommand.ExecuteReader();
                    }
                    catch (SqliteException error)
                    {
                        //Handle error
                        return title;
                    }
                    while (query.Read())
                    {
                        title.Add(query.GetString(0));
                    }
                    db.Close();
                }
                return title;
            }

            count = Grab_Titles().Count;

            List<String> Grab_Dates()
            {
                List<String> title = new List<string>();
                using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                {
                    db.Open();

                    SqliteCommand selectCommand = new SqliteCommand("SELECT title from TitleTable", db);
                    SqliteDataReader query;
                    try
                    {
                        query = selectCommand.ExecuteReader();
                    }
                    catch (SqliteException error)
                    {
                        //Handle error
                        return title;
                    }
                    while (query.Read())
                    {
                        title.Add(query.GetString(0));
                    }
                    db.Close();
                }
                return title;
            }

            for (int i = 0; i < count; i++)
            {
                this.datas.Add(new RepeatsListData() { ProjectName = Grab_Titles().ElementAt(i), ProjectDate = Grab_Dates().ElementAt(i) });
            }

        }
    }
}
