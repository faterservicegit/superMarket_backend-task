using System;
using System.Collections.Generic;
using System.Text;

namespace FaterRasanehMarket.ViewModels.Category
{
    public class TreeViewCategory
    {
        public TreeViewCategory()
        {
            subs = new List<TreeViewCategory>();
        }
        public int id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public List<TreeViewCategory> subs { get; set; }
    }
}
