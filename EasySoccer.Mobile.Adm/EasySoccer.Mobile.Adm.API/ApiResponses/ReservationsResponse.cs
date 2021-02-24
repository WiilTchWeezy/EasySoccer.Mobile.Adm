using System.Collections.Generic;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class ReservationsResponse
    {
        public List<ReservationResponse> Data { get; set; }
        public int Total { get; set; }
    }
}
