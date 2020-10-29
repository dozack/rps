namespace RPS_Modbus
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
        public bool IsMaster { get; set; } = false;

        public ModbusProtocolType ProtocolType = ModbusProtocolType.ASCII;
    }
}
