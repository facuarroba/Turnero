using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TurneroClassLibrary
{
    public static class ConfigManager
    {
        public static String ReadConnectionSetting(string AppName,string keyName)
        {
            try
            {
                string key = AppName + ".Properties.Settings." + keyName;
                string result = ConfigurationManager.ConnectionStrings[key].ToString();
                return result;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static String readStringSetting(String key)
        {
            try
            {
                string result = ConfigurationManager.AppSettings[key];
                return result;
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
