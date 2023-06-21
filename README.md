# Mako-IoT.Device.Services.DataProviders
This component lets you send data (for example from sensors) through mesage bus either periodically or based on an event. See [Messaging sample](https://github.com/CShark-Hub/Mako-IoT.Device.Samples/tree/main/Messaging).

## Usage
Data provider with periodically executed logic ("polling")
```c#
public class MyPollingDataProvider : IDataProvider
{
    private readonly IMySensor _sensor;
    public string Id { get; }
    public event MessageEventHandler DataReceived;
    
    public MyPollingDataProvider(IMySensor sensor)
    {
        Id = nameof(MyPollingDataProvider);
        _sensor = sensor;
    }

    public void GetData()
    {
        var data = _sensor.Read();
        DataReceived?.Invoke(this, new MessageEventArgs(new SensorMessage(data)));
    }
}
```
Event-based Data provider (e.g. for button press event)
```c#
public class MyButtonDataProvider : IDataProvider
{
    public string Id { get; }
    public event MessageEventHandler DataReceived;
    
    public MyButtonDataProvider(IButton button)
    {
        Id = nameof(MyButtonDataProvider);
        button.Press += (s, e) => DataReceived?.Invoke(this, new MessageEventArgs(new ButtonPressedMessage("My button")));
    }

    public void GetData() { }
}
```
Registration in _DeviceBuilder_
```c#
DeviceBuilder.Create()
    .AddMqtt()
    .AddMessageBus()
    .AddWiFi()
    .AddScheduler(o => { })
    .AddDataProviders(o =>
    {
        o.AddDataProvider(typeof(MyPollingDataProvider), nameof(MyPollingDataProvider));
        o.AddDataProvider(typeof(MyButtonDataProvider), nameof(MyButtonDataProvider));
    })
    .AddConfiguration(cfg =>
    {
        cfg.WriteDefault(DataProvidersConfig.SectionName, new DataProvidersConfig
        {
            Providers = new[] { new DataProviderConfig { DataProviderId = "HelloWorldDataProvider", PollInterval = 5000 }}
        });

        cfg.WriteDefault(WiFiConfig.SectionName, new WiFiConfig
        {
            Ssid = "",
            Password = ""
        });

        cfg.WriteDefault(MqttConfig.SectionName, new MqttConfig
        {
            BrokerAddress = "test.mosquitto.org",
            Port = 1883,
            UseTLS = false,
            ClientId = "device1",
            TopicPrefix = "mako-iot-test"
        });
    })
    .Build()
    .Start();
```
### Note
Remember to abstract your hardware (sensors, buttons, etc.) with interfaces (ISensor, IButton, etc.). This allows to write unit tests for data providers.
