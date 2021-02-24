using Newtonsoft.Json;
using System;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class ReservationResponse
    {
        public Guid Id { get; set; }
        public DateTime SelectedDate { get; set; }
        public HourResponse SelectedHourStart { get; set; }
        public HourResponse SelectedHourEnd { get; set; }
        public string SoccerPitchName { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public Guid? UserId { get; set; }
        public long SoccerPitchId { get; set; }
        public int Status { get; set; }
        public string StatusDescription { get; set; }
        public string SoccerPitchColor { get; set; }
        public long SoccerPitchSoccerPitchPlanId { get; set; }
        public int SoccerPitchPlanId { get; set; }

        [JsonIgnore]
        public string ReservationHour
        {
            get
            {
                var dateFormat = string.Empty;
                if (this.SelectedDate != null)
                    dateFormat = this.SelectedDate.ToString("dd/MM/yyyy");
                if (this.SelectedHourStart != null)
                    dateFormat += $" {SelectedHourStart.Hour}:{SelectedHourStart.Minute}";
                if(this.SelectedHourEnd != null)
                    dateFormat += $" - {SelectedHourEnd.Hour}:{SelectedHourEnd.Minute}";

                return dateFormat;
            }
        }

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
