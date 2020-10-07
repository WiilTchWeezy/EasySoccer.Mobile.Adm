using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasySoccer.Mobile.Adm.Infra.Constants
{
    public class Day
    {
        public int DayIndex { get; set; }

        public string DayName { get; set; }

        public static List<Day> Days { get { return GetDays(); }  }
        public static List<string> DaysNames { get { return GetDays().Select(x => x.DayName).ToList(); } }

        public Day()
        {

        }
        private static List<Day> GetDays()
        {
            var days = new List<Day>();
            days.Add(new Day
            {
                DayIndex = 0,
                DayName = "Domingo"
            });
            days.Add(new Day
            {
                DayIndex = 1,
                DayName = "Segunda"
            });
            days.Add(new Day
            {
                DayIndex = 2,
                DayName = "Terça"
            });
            days.Add(new Day
            {
                DayIndex = 3,
                DayName = "Quarta"
            });
            days.Add(new Day
            {
                DayIndex = 4,
                DayName = "Quinta"
            });
            days.Add(new Day
            {
                DayIndex = 5,
                DayName = "Sexta"
            });
            days.Add(new Day
            {
                DayIndex = 6,
                DayName = "Sabado"
            });
            return days;
        }
    }
}
