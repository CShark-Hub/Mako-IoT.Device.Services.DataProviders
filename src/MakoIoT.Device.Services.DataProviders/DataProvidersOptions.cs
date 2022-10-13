using System;
using System.Collections;

namespace MakoIoT.Device.Services.DataProviders
{
    public class DataProvidersOptions
    {
        internal Hashtable DataProviders = new();

        public void AddDataProvider(Type dataProviderType, string id)
        {
            DataProviders.Add(id, dataProviderType);
        }
    }
}
