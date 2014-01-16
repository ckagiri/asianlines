using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Microsoft.WindowsAzure;

namespace Ligi.Infrastructure.Sql.Common
{
    public class ServiceConfigurationSettingConnectionFactory : IDbConnectionFactory
    {
        private readonly object _lockObject = new object();
        private readonly IDbConnectionFactory _parent;
        private Dictionary<string, string> _cachedConnectionStringsMap = new Dictionary<string, string>();

        public ServiceConfigurationSettingConnectionFactory(IDbConnectionFactory parent)
        {
            _parent = parent;
        }

        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            string connectionString = null;
            if (!IsConnectionString(nameOrConnectionString))
            {
                if (!_cachedConnectionStringsMap.TryGetValue(nameOrConnectionString, out connectionString))
                {
                    lock (_lockObject)
                    {
                        if (!_cachedConnectionStringsMap.TryGetValue(nameOrConnectionString, out connectionString))
                        {
                            var connectionStringName = "DbContext." + nameOrConnectionString;
                            var settingValue = CloudConfigurationManager.GetSetting(connectionStringName);
                            if (!string.IsNullOrEmpty(settingValue))
                            {
                                connectionString = settingValue;
                            }

                            if (connectionString == null)
                            {
                                try
                                {
                                    var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
                                    if (connectionStringSettings != null)
                                    {
                                        connectionString = connectionStringSettings.ConnectionString;
                                    }
                                }
                                catch (ConfigurationErrorsException)
                                {
                                }
                            }

                            var immutableDictionary = _cachedConnectionStringsMap
                                .Concat(new[] { new KeyValuePair<string, string>(nameOrConnectionString, connectionString) })
                                .ToDictionary(x => x.Key, x => x.Value);

                            _cachedConnectionStringsMap = immutableDictionary;
                        }
                    }
                }
            }

            if (connectionString == null)
            {
                connectionString = nameOrConnectionString;
            }

            return _parent.CreateConnection(connectionString);
        }

        private static bool IsConnectionString(string connectionStringCandidate)
        {
            return (connectionStringCandidate.IndexOf('=') >= 0);
        }
    }
}
