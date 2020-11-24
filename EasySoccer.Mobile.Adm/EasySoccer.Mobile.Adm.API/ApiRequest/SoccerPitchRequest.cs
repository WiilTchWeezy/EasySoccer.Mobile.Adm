namespace EasySoccer.Mobile.Adm.API.ApiRequest
{
    public class SoccerPitchRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasRoof { get; set; }
        public int NumberOfPlayers { get; set; }
        public SoccerPitchSoccerPitchPlanRequest[] Plans { get; set; }
        public bool Active { get; set; }
        public int SportTypeId { get; set; }

        public int Interval { get; set; }

        public string Color { get; set; }
        public string ImageBase64 { get; set; }
    }

    public class SoccerPitchSoccerPitchPlanRequest
    {
        public int Id { get; set; }
    }
}
