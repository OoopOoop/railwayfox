using System.Configuration;

namespace ToyTrainProject.Shared
{
    public static class AppConfiguration
    {
        public static string UriBase
        {
            get { return ReadSetting("UriBase"); }
            set { AddUpdateAppSettings("UriBase", value); }
        }

        public static string SubscriptionKey
        {
            get { return ReadSetting("SubscriptionKey"); }
            set { AddUpdateAppSettings("SubscriptionKey", value); }
        }

        public static class ComputerVisionAPI
        {
            public static class AnalyzeImage
            {
                public static string visualFeatures => ReadSetting("ComputerVisionAPI.AnalyzeImage.visualFeatures");
                public static string details => ReadSetting("ComputerVisionAPI.AnalyzeImage.details");
                public static string language => ReadSetting("ComputerVisionAPI.AnalyzeImage.language");
            }

            public static class DescribeImage
            {
                public static int maxCandidates => int.Parse(ReadSetting("ComputerVisionAPI.DescribeImage.maxCandidates"));
            }

            public static class OCR
            {
                public static string language => ReadSetting("ComputerVisionAPI.OCR.language");
                public static bool detectOrientation => bool.Parse(ReadSetting("ComputerVisionAPI.OCR.detectOrientation"));
            }
        }

        private static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                System.Windows.MessageBox.Show("Error writing app settings");
            }
        }

        private static string ReadSetting(string key, string defaultValue = "")
        {
            string returnString = defaultValue;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings[key] == null)
                {
                    AddUpdateAppSettings(key, defaultValue);
                    return defaultValue;
                }
                else
                {
                    returnString = appSettings[key];
                }
            }
            catch (ConfigurationErrorsException)
            {
                System.Windows.MessageBox.Show("Error reading app settings");
            }

            return returnString;
        }
    }
}