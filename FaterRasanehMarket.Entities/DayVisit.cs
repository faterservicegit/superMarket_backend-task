using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FaterRasanehMarket.Entities
{
   public class DayVisit
    {
        [Key]
        public DateTime DateTime { get; set; }
        public int VisitCount { get; set; }
    }
}
