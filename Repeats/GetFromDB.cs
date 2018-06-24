using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeats
{
    public class GetFromDB
    {
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
    }
}
