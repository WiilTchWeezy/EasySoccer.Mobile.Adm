using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class UserInfoResponse
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public long? CompanyId { get; set; }
    }
}
