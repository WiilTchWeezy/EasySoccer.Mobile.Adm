using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiRequest
{
    public class PatchCompanyInfoRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CNPJ { get; set; }
        public string CompleteAddress { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
