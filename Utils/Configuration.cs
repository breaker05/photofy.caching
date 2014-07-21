using System;
using System.Configuration;

namespace Photofy.Caching.Utils
{
    public static class Configuration
    {
        public static T GetSetting<T>(string key) where T : IConvertible
        {
            var obj = GetSetting(key);
            if (obj == null || obj == string.Empty) return default(T);
            return (T)System.Convert.ChangeType(obj, typeof(T));
        }
        public static string GetSetting(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key].ToString();
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}
