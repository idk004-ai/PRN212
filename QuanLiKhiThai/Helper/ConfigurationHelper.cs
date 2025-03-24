using QuanLiKhiThai.DAO.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QuanLiKhiThai.Helper
{
    public class ConfigurationHelper
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.config"); // get configuration from AppConfig.xml

        private readonly IValidationLogger _logger;
        private static IValidationLogger? _staticLogger;

        public ConfigurationHelper(IValidationLogger logger)
        {
            _logger = logger;
            _staticLogger = logger;
        }

        /// <summary>
        /// Logs configuration error
        /// </summary>
        /// <param name="message">Error message</param>
        public void LogConfigurationError(string message)
        {
            _logger?.LogValidationError(message);
        }

        /// <summary>
        /// static method to log configuration error
        /// </summary>
        /// <param name="message">Error message</param>
        public static void LogStaticConfigurationError(string message)
        {
            _staticLogger?.LogValidationError(message);

            // Luôn đảm bảo ghi ra debug để phòng hờ logger chưa được khởi tạo
            System.Diagnostics.Debug.WriteLine($"[CONFIG ERROR] {message}");
        }

        static ConfigurationHelper()
        {
            // Creat default config file if it doesn't exist
            if (!File.Exists(ConfigFilePath))
            {
                LogStaticConfigurationError("Configuration file not found. Creating default configuration file...");
                CreateDefaultConfigFile();
            }
        }

        /// <summary>
        /// Create default configuration file
        /// </summary>
        private static void CreateDefaultConfigFile()
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                // Declares the XML version and the encoding (UTF-8)
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(xmlDeclaration);

                // Create root element
                XmlElement root = doc.CreateElement("configuration");
                doc.AppendChild(root);

                // create appSettings element
                XmlElement appSettings = doc.CreateElement("appSettings");
                root.AppendChild(appSettings);

                // Add default settings
                AddSetting(doc, appSettings, "EmailUsername", "minhkhoilenhat04@gmail.com");
                AddSetting(doc, appSettings, "EmailPassword", "msfi tdvq dyru czdg");
                AddSetting(doc, appSettings, "SmtpServer", "smtp.gmail.com");
                AddSetting(doc, appSettings, "SmtpPort", "587");
                AddSetting(doc, appSettings, "EnableSsl", "true");
                AddSetting(doc, appSettings, "SenderName", "Vehicle Inspection System");

                // Add expiration service settings
                AddSetting(doc, appSettings, "ExpirationCheckIntervalMinutes", "60");
                AddSetting(doc, appSettings, "InspectionExpirationHours", "24");

                // Add more default settings
                AddSetting(doc, appSettings, "DefaultPageSize", "10");
                AddSetting(doc, appSettings, "ApplicationName", "Vehicle Emission Control System");

                doc.Save(ConfigFilePath);
            }
            catch (Exception ex)
            {
                LogStaticConfigurationError($"Failed to create default configuration file: {ex.Message}");
            }
        }

        private static void AddSetting(XmlDocument doc, XmlElement parent, string key, string value)
        {
            XmlElement element = doc.CreateElement("add");
            element.SetAttribute("key", key);
            element.SetAttribute("value", value);
            parent.AppendChild(element);
        }

        public static string GetAppSetting(string key)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigFilePath);

                XmlNode? node = doc.SelectSingleNode($"//appSettings/add[@key='{key}']");
                if (node != null)
                {
                    return node.Attributes["value"].Value;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void SetAppSetting(string key, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigFilePath);

                XmlNode? node = doc.SelectSingleNode($"//appSettings/add[@key='{key}']");
                if (node != null)
                {
                    node.Attributes["value"].Value = value;
                }
                else
                {
                    // Create new node if it doesn't exist
                    XmlNode appSettings = doc.SelectSingleNode("//appSettings");
                    if (appSettings != null)
                    {
                        XmlElement newNode = doc.CreateElement("add");
                        newNode.SetAttribute("key", key);
                        newNode.SetAttribute("value", value);
                        appSettings.AppendChild(newNode);
                    }
                }

                doc.Save(ConfigFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save app setting: {ex.Message}");
            }
        }
    }
}
