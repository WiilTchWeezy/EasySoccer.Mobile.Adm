using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiRequest
{
    public class SoccerPitchPlanRequest
    {
        public int id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }
        public string Description { get; set; }
    }
}
