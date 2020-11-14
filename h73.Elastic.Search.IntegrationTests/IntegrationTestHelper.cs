using Microsoft.Extensions.Configuration;

namespace eSmart.Elastic.Search.IntegrationTests
{
    public static class IntegrationTestHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot()
        {            
            return new ConfigurationBuilder()
                .AddJsonFile("application.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static ElasticsearchSettings GetApplicationConfiguration()
        {
            var configuration = new ElasticsearchSettings();

            var iConfig = GetIConfigurationRoot();

            iConfig
                .GetSection("elasticsearch")
                .Bind(configuration);

            return configuration;
        }
    }
}