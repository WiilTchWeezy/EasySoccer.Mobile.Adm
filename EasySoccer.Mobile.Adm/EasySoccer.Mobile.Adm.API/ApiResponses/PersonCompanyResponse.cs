using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class PersonCompanyResponse
    {
        public int Total { get; set; }
        public List<PersonCompany> Data { get; set; }
    }

    public class PersonCompany
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
