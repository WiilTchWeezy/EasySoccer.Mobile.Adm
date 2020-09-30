using System.Collections.Generic;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class CompanyInfoResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CompleteAddress { get; set; }
        public string CNPJ { get; set; }
        public string Logo { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string City { get; set; }
        public List<CompanyDaySchedule> CompanySchedules { get; set; }
    }
}
