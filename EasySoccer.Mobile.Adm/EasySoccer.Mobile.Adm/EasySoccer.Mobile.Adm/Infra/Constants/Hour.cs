using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasySoccer.Mobile.Adm.Infra.Constants
{
    public class Hour
    {
        public int Index { get; set; }

        public string Description { get; set; }

        public static List<Hour> Hours { get { return GetHours(); } }
        public static List<string> HoursDescriptions { get { return GetHours().Select(x => x.Description).ToList(); } }

        private static List<Hour> GetHours()
        {
            var hours = new List<Hour>();
            for (int i = 0; i < 24; i++)
            {
                hours.Add(new Hour
                {
                    Index = i,
                    Description = i > 9 ? $"{i}:00" : $"0{i}:00"
                });
            }
            return hours;
        }

        public static List<string> GetHoursGreaterThan(int hour)
        {
            return Hours.Where(x => x.Index > hour).Select(x => x.Description).ToList();
        }
    }
}
