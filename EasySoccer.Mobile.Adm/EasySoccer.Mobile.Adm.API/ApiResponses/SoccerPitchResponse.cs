using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class SoccerPitchResponse
    {
        public long Id { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public bool HasRoof { get; set; }
        public string Name { get; set; }
        public int NumberOfPlayers { get; set; }
        public List<PlansInfoResponse> Plans { get; set; }
        public int SportTypeId { get; set; }
        public string SportTypeName { get; set; }
        public int? Interval { get; set; }
        public string ImageName { get; set; }
        public string Color { get; set; }

        [JsonIgnore]
        public DelegateCommand<SoccerPitchResponse> EditSoccerPitchCommand { get; set; }
    }
}
