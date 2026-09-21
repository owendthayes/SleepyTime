using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepyTime_2._0
{
    internal class PresetItem
    {
        public string Name { get; set; }
        public string Action { get; set; }
        public string Repeat { get; set; }
        public TimeSpan Time { get; set; }
        public string Days { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastRun { get; set; }

        public PresetItem(string name, string action, string repeat, TimeSpan time, string days, bool enabled, DateTime lastRun)
        {
            Name = name;
            Action = action;
            Repeat = repeat;
            Time = time;
            Days = days;
            Enabled = enabled;
            LastRun = lastRun;
        }

        public string toString()
        {
            return $"{Name}|{Action}|{Repeat}|{Time}|{Days}|{Enabled}|{LastRun}";
        }
    }
}
