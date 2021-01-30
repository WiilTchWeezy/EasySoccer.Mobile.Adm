using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class CompanyFinancialInfo
    {
        public long Id { get; set; }

        public long CompanyId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ExpiresDate { get; set; }

        public decimal Value { get; set; }

        public string Transaction { get; set; }

        public bool Paid { get; set; }

        public int FinancialPlan { get; set; }

    }
}
