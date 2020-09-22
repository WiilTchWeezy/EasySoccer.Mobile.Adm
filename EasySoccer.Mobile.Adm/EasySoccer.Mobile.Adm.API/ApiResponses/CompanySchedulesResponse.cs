using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class CompanySchedulesResponse
    {
        public ObservableCollection<CompanySchedulesEventResponse> Events { get; set; }
        public string Hour { get; set; }
        public TimeSpan HourSpan { get; set; }

    }
}
