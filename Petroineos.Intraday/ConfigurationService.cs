using Microsoft.Extensions.Configuration;

namespace Petroineos.Intraday
{
    public interface IConfigurationService
    {
        string ReportsLocation { get; }
        int ReportsFrequency { get; }
    }

    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(IConfiguration configuration)
        {
            ReportsLocation = configuration.GetValue("ReportsLocation", "Reports");
            ReportsFrequency = (int)(configuration.GetValue<float>("ReportsFrequency", 1) * 60000);
        }

        public string ReportsLocation { get; }
        public int ReportsFrequency { get; }
    }
}