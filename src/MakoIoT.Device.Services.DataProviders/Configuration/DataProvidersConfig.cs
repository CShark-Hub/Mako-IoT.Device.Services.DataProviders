namespace MakoIoT.Device.Services.DataProviders.Configuration
{
    public class DataProvidersConfig
    {
        public DataProviderConfig[] Providers { get; set; }

        public static string SectionName => "DataProviders";
    }

    public class DataProviderConfig
    {
        public string DataProviderId { get; set; }
        public int PollInterval { get; set; }
    }
}
