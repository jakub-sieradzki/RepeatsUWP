using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace DataAccessLibrary
{
    public class DataAccess
    {
        public static void CreateDatabase()
        {
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT EXISTS TitleTable " +
                    "(id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "title NVARCHAR(2048) NULL, " +
                    "TableName NVARCHAR(2048) NULL, " +
                    "CreateDate NVARCHAR(2048) NULL, " +
                    "IsEnabled NVARCHAR(2048) NULL, " +
                    "Avatar NVARCHAR(2048) NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();

                db.Close();
            }
        }

        public static void CreateTable(string date)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT EXISTS " + date + 
                    " (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "question NVARCHAR(2048) NULL, " +
                    "answer NVARCHAR(2048) NULL, " +
                    "image NVARCHAR(2048) NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();

                db.Close();
            }
        }

        public static void SaveToTableSet(string question, string answer, string tag, string date)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO " + date + " VALUES (NULL, @question, @answer, @image);";
                insertCommand.Parameters.AddWithValue("@question", question);
                insertCommand.Parameters.AddWithValue("@answer", answer);
                insertCommand.Parameters.AddWithValue("@image", tag);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }

        public static void SaveToTitleTable(string getname, string date, string realDate, string TAG)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();

                SqliteCommand insertCommand2 = new SqliteCommand();
                insertCommand2.Connection = db;

                insertCommand2.CommandText = "INSERT INTO TitleTable VALUES (NULL, @title, @TableName, @CreateDate, @IsEnabled, @Avatar);";
                insertCommand2.Parameters.AddWithValue("@title", getname);
                insertCommand2.Parameters.AddWithValue("@TableName", date);
                insertCommand2.Parameters.AddWithValue("@CreateDate", realDate);
                insertCommand2.Parameters.AddWithValue("@IsEnabled", "true");
                insertCommand2.Parameters.AddWithValue("@Avatar", TAG);

                insertCommand2.ExecuteReader();

                db.Close();
            }
        }

        public static void DropTable(string tablename)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();

                String tableCommand2 = "DROP TABLE " + tablename;
                SqliteCommand createTable2 = new SqliteCommand(tableCommand2, db);

                createTable2.ExecuteReader();

                db.Close();
            }
        }

        public static void DelFromTitleTable(string tablename)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();

                SqliteCommand insertCommand3 = new SqliteCommand();
                insertCommand3.Connection = db;

                insertCommand3.CommandText = "DELETE FROM TitleTable WHERE TableName=" + "\"" + tablename + "\"";

                insertCommand3.ExecuteReader();

                db.Close();
            }
        }

        public static void UpdateTable(string TABLE, string WHAT, string WHERE)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();

                string tableCommand;

                if (WHERE != "")
                {
                    tableCommand = "UPDATE " + TABLE + " SET " + WHAT + " WHERE " + WHERE;
                }
                else
                {
                    tableCommand = "UPDATE " + TABLE + " SET " + WHAT;
                }


                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
                 
                db.Close();
            }
        }

        public static List<string> GrabData(string FROM, string WHAT)
        {
            List<string> data = new List<string>();
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

        public static IList<string> GrabTitles(string FROM, string WHAT)
        {
            IList<string> titles = new List<string>();

            using (SqliteConnection db = new SqliteConnection("Filename=Repeats.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT " + WHAT + " from " + FROM + " WHERE " + "IsEnabled='true'", db);
                SqliteDataReader query;

                query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    titles.Add(query.GetString(0));
                }
                db.Close();
            }
            return titles;
        }
    }
}