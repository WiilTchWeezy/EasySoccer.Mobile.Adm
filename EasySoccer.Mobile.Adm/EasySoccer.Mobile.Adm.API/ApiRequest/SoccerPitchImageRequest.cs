using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiRequest
{
    public class SoccerPitchImageRequest
    {
        public long SoccerPitchId { get; set; }
        public string ImageBase64 { get; set; }
    }
}
