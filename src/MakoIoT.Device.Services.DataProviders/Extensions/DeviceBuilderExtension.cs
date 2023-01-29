using MakoIoT.Device.Services.Interface;
using nanoFramework.DependencyInjection;

namespace MakoIoT.Device.Services.DataProviders.Extensions
{
    public delegate void DataProvidersConfigurator(DataProvidersOptions options);

    public static class DeviceBuilderExtension
    {
        public static IDeviceBuilder AddDataProviders(this IDeviceBuilder builder, DataProvidersConfigurator configurator)
        {
            var options = new DataProvidersOptions();
            configurator(options);
            builder.Services.AddSingleton(typeof(DataProvidersOptions), options);

            builder.DeviceStarting += Builder_DeviceStarting;

            return builder;
        }

        private static void Builder_DeviceStarting(IDevice sender)
        {
            var dp = (DataPublisher)ActivatorUtilities.CreateInstance(sender.ServiceProvider, typeof(DataPublisher));
            dp.InitializeDataProviders();
        }
    }
}
