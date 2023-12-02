using System;
using System.Collections;
using MakoIoT.Device.Services.DataProviders.Configuration;
using MakoIoT.Device.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace MakoIoT.Device.Services.DataProviders
{
    public class DataPublisher
    {
        private readonly IScheduler _scheduler;
        private readonly IMessageBus _messageBus;
        private readonly IConfigurationService _config;
        private readonly ILog _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Hashtable _dataProviders = new();

        public DataPublisher(IScheduler scheduler, IMessageBus messageBus, IConfigurationService config, ILog logger, DataProvidersOptions options, IServiceProvider serviceProvider)
        {
            _scheduler = scheduler;
            _messageBus = messageBus;
            _config = config;
            _logger = logger;
            _serviceProvider = serviceProvider;
            RegisterDataProviders(options);
        }

        private void RegisterDataProviders(DataProvidersOptions options)
        {
            foreach (var p in options.DataProviders.Keys)
            {
                _logger.Trace($"Adding data provider {p}");
                var provider = (IDataProvider)ActivatorUtilities.CreateInstance(_serviceProvider, (Type)options.DataProviders[p]);
                _dataProviders.Add(p, provider);
                provider.DataReceived += ProviderOnDataReceived;
            }
        }

        private void ProviderOnDataReceived(object sender, MessageEventArgs e)
        {
            _logger.Trace($"Message {e.Message.MessageType} received from data provider");
            _messageBus.Publish(e.Message);
        }

        public void InitializeDataProviders()
        {
            var dpc = (DataProvidersConfig)_config.GetConfigSection(DataProvidersConfig.SectionName, typeof(DataProvidersConfig));

            foreach (var providerConfig in dpc.Providers)
            {
                try
                {
                    _logger.Trace(
                        $"DataProviderId found in config: {providerConfig.DataProviderId}, {providerConfig.PollInterval}");
                    if (_dataProviders.Contains(providerConfig.DataProviderId))
                    {
                        var provider = (IDataProvider)_dataProviders[providerConfig.DataProviderId];
                        _scheduler.Start(() => provider.GetData(), providerConfig.PollInterval, provider.Id);
                    }
                    else
                    {
                        _logger.Warning($"Provider implementation {providerConfig.DataProviderId} not found");
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"Error initializing data provider {providerConfig?.DataProviderId}", e);
                }
            }
        }
    }
}
