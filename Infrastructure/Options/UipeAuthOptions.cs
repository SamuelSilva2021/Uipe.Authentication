using Authentication.Infrastructure.Configuration;

namespace Authentication.Infrastructure.Options
{
    public class UipeAuthOptions
    {
        public TokenConfigurationsOptions TokenConfigurations { get; set; } = new TokenConfigurationsOptions();
        public string AuthConnectionString { get; set; } = string.Empty;
        public int SystemId { get; set; }
    }
}
