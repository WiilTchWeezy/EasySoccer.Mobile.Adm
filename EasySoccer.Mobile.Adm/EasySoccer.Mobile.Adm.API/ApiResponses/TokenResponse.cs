using System;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
