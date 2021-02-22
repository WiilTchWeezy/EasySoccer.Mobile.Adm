using System;

namespace EasySoccer.Mobile.Adm.API.ApiRequest
{
    public class ChangeStatusRequest
    {
        public Guid ReservationId { get; set; }
        public int Status { get; set; }
    }
}
