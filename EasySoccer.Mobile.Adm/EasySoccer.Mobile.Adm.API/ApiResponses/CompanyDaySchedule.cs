using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class CompanyDaySchedule
    {
        public long CompanyId { get; set; }
        public int Day { get; set; }
        public long FinalHour { get; set; }
        public long StartHour { get; set; }
        public bool WorkOnThisDay { get; set; }
    }
}
