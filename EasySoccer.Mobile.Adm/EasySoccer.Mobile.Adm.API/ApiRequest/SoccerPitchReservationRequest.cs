using System;

namespace EasySoccer.Mobile.Adm.API.ApiRequest
{
    public class SoccerPitchReservationRequest
    {
        public long SoccerPitchId { get; set; }
        public DateTime SelectedDate { get; set; }
        public TimeSpan HourStart { get; set; }
        public TimeSpan HourEnd { get; set; }
        public int SoccerPitchPlanId { get; set; }
        public int Application { get; set; } = 2;
    }
}
