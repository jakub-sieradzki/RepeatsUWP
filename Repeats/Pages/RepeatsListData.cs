﻿using System;
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

            List<string> Grab_Titles = GetFromDB.GrabData("TitleTable", "title");
            List<string> Grab_Dates = GetFromDB.GrabData("TitleTable", "CreateDate");
            List<string> Grab_Names = GetFromDB.GrabData("TitleTable", "TableName");

            count = Grab_Titles.Count;

            for (int i = 0; i < count; i++)
            {
                this.datas.Add(new RepeatsListData() { ProjectName = Grab_Titles.ElementAt(i), ProjectDate = Grab_Dates.ElementAt(i), TableName = Grab_Names.ElementAt(i) });
            }

        }
    }
}
