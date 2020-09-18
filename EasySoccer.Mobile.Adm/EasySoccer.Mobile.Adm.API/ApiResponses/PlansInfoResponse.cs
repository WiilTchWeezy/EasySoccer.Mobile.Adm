using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class PlansInfoResponse
    {
        public string PlanName { get; set; }
        public decimal Value { get; set; }
        public int MaxInstallments { get; set; }
        public int PlanId { get; set; }
    }
}
