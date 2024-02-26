namespace Authentication.Infrastructure.Configuration
{
    public class TokenConfigurationsOptions
    {
        public string Key { get; set; }
        public string Audience { get; set; } = "Uipe Internal Apis";
        public string Issuer { get; set; } = "Uipe.Auth";

        public int Seconds { get; set; } = 360;
    }
}
