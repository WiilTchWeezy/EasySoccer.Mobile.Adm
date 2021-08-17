using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class PlansResponseData
    {
        public List<PlansResponse> Data { get; set; }
        public int Total { get; set; }
    }
    public class PlansResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }

        [JsonIgnore]
        public DelegateCommand<PlansResponse> EditPlanCommand { get; set; }
    }
}
