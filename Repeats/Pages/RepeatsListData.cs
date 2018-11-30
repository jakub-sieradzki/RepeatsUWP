using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DataAccessLibrary;

namespace Repeats.Pages
{
    public class RepeatsListData
    {
        public string ProjectName { get; set; }
        public string ProjectDate { get; set; }
        public string TableName { get; set; }
        public object IsENABLED { get; set; }
        public string avatar { get; set; }
        public string avatarTag { get; set; }

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

            List<string> Grab_Titles = DataAccess.GrabData("TitleTable", "title");
            List<string> Grab_Dates = DataAccess.GrabData("TitleTable", "CreateDate");
            List<string> Grab_Names = DataAccess.GrabData("TitleTable", "TableName");
            List<string> Grab_Enabled = DataAccess.GrabData("TitleTable", "IsEnabled");
            List<string> Grab_Avatars = DataAccess.GrabData("TitleTable", "Avatar");
            count = Grab_Titles.Count;

            for (int i = 0; i < count; i++)
            {
                this.datas.Add(new RepeatsListData()
                {
                    ProjectName = Grab_Titles.ElementAt(i),
                    ProjectDate = Grab_Dates.ElementAt(i),
                    TableName = Grab_Names.ElementAt(i),
                    IsENABLED = Grab_Enabled.ElementAt(i),
                    avatar = Grab_Avatars.ElementAt(i),
                    avatarTag = Grab_Avatars.ElementAt(i)
                });
            }
        }
    }
}
