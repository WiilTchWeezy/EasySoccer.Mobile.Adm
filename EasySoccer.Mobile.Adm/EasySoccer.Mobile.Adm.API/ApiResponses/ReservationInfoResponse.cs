using Newtonsoft.Json;
using System;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class ReservationInfoResponse
    {
        public Guid Id { get; set; }
        public int? Interval { get; set; }
        public string Note { get; set; }
        public Guid? PersonId { get; set; }
        public string PersonName { get; set; }
        public string PersonPhone { get; set; }
        public DateTime SelectedDateStart { get; set; }
        public DateTime SelectedDateEnd { get; set; }
        public long SoccerPitchId { get; set; }
        public string SoccerPitchName { get; set; }
        public int SoccerPitchPlanId { get; set; }
        public string SoccerPitchPlanName { get; set; }
        public string SoccerPitchPlanDescription { get; set; }
        public string SoccerPitchImage { get; set; }
        public string CompanyImage { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string SoccerPitchSportType { get; set; }
        public string StatusDescription { get; set; }
        public int Status { get; set; }

        [JsonIgnore]
        public string StatusColor
        {
            get
            {
                string color = string.Empty;
                switch (Status)
                {
                    case 1:
                        color = "#f0ad4e";
                        break;
                    case 2:
                        color = "#d9534f";
                        break;
                    case 3:
                        color = "#5cb85c";
                        break;
                    case 4:
                        color = "#0275d8";
                        break;
                    default:
                        break;
                }
                return color;
            }
        }
    }
}
