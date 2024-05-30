using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTrackingSystem.Models
{
    public class ScheduleTestModel
    {
        public string TestType { get; set; }
        public string TestDate { get; set; }
        public string StartTime { get; set; }
        public int Duration { get; set; }
        public string SelectedLink { get; set; }
    }
}
