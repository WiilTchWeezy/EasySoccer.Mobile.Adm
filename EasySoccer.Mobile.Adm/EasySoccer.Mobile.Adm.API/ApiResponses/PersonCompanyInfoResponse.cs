using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class PersonCompanyInfoResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<ReservationsInfoResponse> Reservations { get; set; }
    }

    public class ReservationsInfoResponse
    {
        public Guid Id { get; set; }
        public long SoccerPitchId { get; set; }
        public string SoccerPitchName { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime SelectedDateStart { get; set; }
        public DateTime SelectedDateEnd { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
