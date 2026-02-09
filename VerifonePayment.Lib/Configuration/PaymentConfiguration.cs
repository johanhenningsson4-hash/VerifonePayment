using System;
using System.Configuration;

namespace VerifonePayment.Lib.Configuration
{
    /// <summary>
    /// Configuration settings for Verifone payment system
    /// </summary>
    public class PaymentConfiguration
    {
        #region "Constants"

        private const string DEVICE_IP_ADDRESS_KEY = "DeviceIpAddress";
        private const string DEVICE_CONNECTION_TYPE_KEY = "DeviceConnectionType";
        private const string DEFAULT_USERNAME_KEY = "DefaultUsername";
        private const string DEFAULT_PASSWORD_KEY = "DefaultPassword";
        private const string DEFAULT_SHIFT_NUMBER_KEY = "DefaultShiftNumber";
        private const string LOG_FILE_PATH_KEY = "LogFilePath";
        private const string DELETE_LOG_FILE_KEY = "DeleteLogFile";
        private const string CONNECTION_TIMEOUT_KEY = "ConnectionTimeoutSeconds";
        private const string TRANSACTION_TIMEOUT_KEY = "TransactionTimeoutSeconds";

        #endregion

        #region "Properties"

        /// <summary>
        /// Device IP address for TCP/IP connection
        /// </summary>
        public string DeviceIpAddress { get; private set; }

        /// <summary>
        /// Device connection type (default: tcpip)
        /// </summary>
        public string DeviceConnectionType { get; private set; }

        /// <summary>
        /// Default username for device login
        /// </summary>
        public string DefaultUsername { get; private set; }

        /// <summary>
        /// Default password for device login
        /// </summary>
        public string DefaultPassword { get; private set; }

        /// <summary>
        /// Default shift number for device login
        /// </summary>
        public string DefaultShiftNumber { get; private set; }

        /// <summary>
        /// Log file path (if null, uses temp directory)
        /// </summary>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Whether to delete log file on startup
        /// </summary>
        public bool DeleteLogFile { get; private set; }

        /// <summary>
        /// Connection timeout in seconds
        /// </summary>
        public int ConnectionTimeoutSeconds { get; private set; }

        /// <summary>
        /// Transaction timeout in seconds
        /// </summary>
        public int TransactionTimeoutSeconds { get; private set; }

        #endregion

        #region "Constructor"

        /// <summary>
        /// Initializes configuration from app settings
        /// </summary>
        public PaymentConfiguration()
        {
            LoadConfiguration();
        }

        /// <summary>
        /// Initializes configuration with custom device IP (overrides config file)
        /// </summary>
        /// <param name="deviceIpAddress">Device IP address</param>
        public PaymentConfiguration(string deviceIpAddress)
        {
            if (string.IsNullOrWhiteSpace(deviceIpAddress))
                throw new ArgumentException("Device IP address cannot be null or empty", nameof(deviceIpAddress));

            LoadConfiguration();
            DeviceIpAddress = deviceIpAddress;
        }

        #endregion

        #region "Private Methods"

        /// <summary>
        /// Loads configuration from app.config
        /// </summary>
        private void LoadConfiguration()
        {
            DeviceIpAddress = GetConfigValue(DEVICE_IP_ADDRESS_KEY, "127.0.0.1");
            DeviceConnectionType = GetConfigValue(DEVICE_CONNECTION_TYPE_KEY, "tcpip");
            DefaultUsername = GetConfigValue(DEFAULT_USERNAME_KEY, "username");
            DefaultPassword = GetConfigValue(DEFAULT_PASSWORD_KEY, "password");
            DefaultShiftNumber = GetConfigValue(DEFAULT_SHIFT_NUMBER_KEY, "shift");
            LogFilePath = GetConfigValue(LOG_FILE_PATH_KEY, null);
            DeleteLogFile = GetConfigValueAsBool(DELETE_LOG_FILE_KEY, false);
            ConnectionTimeoutSeconds = GetConfigValueAsInt(CONNECTION_TIMEOUT_KEY, 30);
            TransactionTimeoutSeconds = GetConfigValueAsInt(TRANSACTION_TIMEOUT_KEY, 60);
        }

        /// <summary>
        /// Gets configuration value as string with default fallback
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value or default</returns>
        private string GetConfigValue(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        /// <summary>
        /// Gets configuration value as boolean with default fallback
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found or invalid</param>
        /// <returns>Configuration value or default</returns>
        private bool GetConfigValueAsBool(string key, bool defaultValue)
        {
            var value = GetConfigValue(key, defaultValue.ToString());
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        /// <summary>
        /// Gets configuration value as integer with default fallback
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found or invalid</param>
        /// <returns>Configuration value or default</returns>
        private int GetConfigValueAsInt(string key, int defaultValue)
        {
            var value = GetConfigValue(key, defaultValue.ToString());
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Validates the current configuration
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(DeviceIpAddress) &&
                   !string.IsNullOrWhiteSpace(DeviceConnectionType) &&
                   !string.IsNullOrWhiteSpace(DefaultUsername) &&
                   !string.IsNullOrWhiteSpace(DefaultPassword) &&
                   !string.IsNullOrWhiteSpace(DefaultShiftNumber) &&
                   ConnectionTimeoutSeconds > 0 &&
                   TransactionTimeoutSeconds > 0;
        }

        /// <summary>
        /// Gets a summary of the current configuration (without sensitive data)
        /// </summary>
        /// <returns>Configuration summary</returns>
        public string GetConfigurationSummary()
        {
            return $"Device: {DeviceIpAddress}, Connection: {DeviceConnectionType}, " +
                   $"Username: {DefaultUsername}, Shift: {DefaultShiftNumber}, " +
                   $"Connection Timeout: {ConnectionTimeoutSeconds}s, " +
                   $"Transaction Timeout: {TransactionTimeoutSeconds}s, " +
                   $"Delete Log: {DeleteLogFile}";
        }

        #endregion
    }
}