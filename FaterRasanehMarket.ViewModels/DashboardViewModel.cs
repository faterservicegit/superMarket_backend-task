using System;
using System.Collections.Generic;

namespace FaterRasanehMarket.ViewModels
{
    public class DashboardViewModel
    {
        public int UsersCount { get; set; }
        public int ProductsCount { get; set; }
        public int VisitsCount { get; set; }
        public long TodaySell { get; set; }
    }
    public class VisitsStatisticsViewModel
    {
        public List<int> VisistsCount { get; set; } = new List<int>();
        public List<int> ProductsCount { get; set; } = new List<int>();
        public List<int> Days { get; set; } = new List<int>();
        public string Month { get; set; }
        public string Year { get; set; }

    }
}
