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
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string City { get; set; }
        public List<CompanyDaySchedule> CompanySchedules { get; set; }
    }
}
