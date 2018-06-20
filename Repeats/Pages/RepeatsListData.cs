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
        public string TableName { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var bi = (RepeatsListData)obj;
            return this.TableName == bi.TableName;
        }

        public override int GetHashCode()
        {
            int a = Int32.Parse(TableName);
            return a;
        }
    }

    public class MainPageDataModel
    {
        private ObservableCollection<RepeatsListData> datas = new ObservableCollection<RepeatsListData>();
        public ObservableCollection<RepeatsListData> Datas { get { return this.datas; } }
        public MainPageDataModel()
        {
            int count;

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

                    SqliteCommand selectCommand = new SqliteCommand("SELECT CreateDate from TitleTable", db);
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

            List<String> Grab_Names()
            {
                List<String> name = new List<string>();
                using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
                {
                    db.Open();

                    SqliteCommand selectCommand = new SqliteCommand("SELECT TableName from TitleTable", db);
                    SqliteDataReader query;
                    try
                    {
                        query = selectCommand.ExecuteReader();
                    }
                    catch (SqliteException error)
                    {
                        //Handle error
                        return name;
                    }
                    while (query.Read())
                    {
                        name.Add(query.GetString(0));
                    }
                    db.Close();
                }
                return name;
            }

            for (int i = 0; i < count; i++)
            {
                this.datas.Add(new RepeatsListData() { ProjectName = Grab_Titles().ElementAt(i), ProjectDate = Grab_Dates().ElementAt(i), TableName = Grab_Names().ElementAt(i) });
            }

        }
    }
}
