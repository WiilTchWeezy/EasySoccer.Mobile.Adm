using System;
using System.Collections.Generic;
using System.Text;


namespace EasySoccer.Mobile.Adm.API.ApiRequest
{
    public class PersonCompanyRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
