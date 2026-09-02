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
        public bool ReminderSent { get; set; }

        public ScheduleItem(string action, DateTime date, TimeSpan time, string reminder, bool reminderSent)
        {
            Action = action;
            Date = date;
            Time = time;
            Reminder = reminder;
            ReminderSent = reminderSent;
        }

        public string toString()
        {
            return $"{Action}|{Date}|{Time}|{Reminder}";
        }
    }
}
