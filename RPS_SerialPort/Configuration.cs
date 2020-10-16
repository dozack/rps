namespace RPS_SerialPort
{
    /// <summary>
    /// Configuration structure for OSI layers
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Serial port name
        /// </summary>
        public string PortName { get; set; } = "";

        /// <summary>
        /// Serial port baud rate
        /// </summary>
        public int BaudRate { get; set; } = 0;

        /// <summary>
        /// Flag for application layer, true for transmitter, false for receiver
        /// </summary>
        public bool IsServer { get; set; } = false;

        /// <summary>
        /// Interval in milliseconds for application layer value update
        /// </summary>
        public int AppUpdateInterval { get; set; } = 100;
    }
}
