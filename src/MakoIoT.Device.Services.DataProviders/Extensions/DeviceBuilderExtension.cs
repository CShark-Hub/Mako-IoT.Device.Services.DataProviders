using MakoIoT.Device.Services.DependencyInjection;
using MakoIoT.Device.Services.Interface;

namespace MakoIoT.Device.Services.DataProviders.Extensions
{
    public delegate void DataProvidersConfigurator(DataProvidersOptions options);

    public static class DeviceBuilderExtension
    {
        public static IDeviceBuilder AddDataProviders(this IDeviceBuilder builder, DataProvidersConfigurator configurator)
        {
            var options = new DataProvidersOptions();
            configurator(options);
            DI.RegisterInstance(typeof(DataProvidersOptions), options);

            builder.DeviceStarting += Builder_DeviceStarting;

            return builder;
        }

        private static void Builder_DeviceStarting(object sender, System.EventArgs e)
        {
            var dp = (DataPublisher)DI.BuildUp(typeof(DataPublisher));
            dp.InitializeDataProviders();
        }
    }
}
