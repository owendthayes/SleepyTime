using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepyTime_2._0
{
    class ScheduleItem
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Reminder { get; set; }

        public ScheduleItem(string action, DateTime date, TimeSpan time, string reminder)
        {
            Action = action;
            Date = date;
            Time = time;
            Reminder = reminder;
        }
    }
}
