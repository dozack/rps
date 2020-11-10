using System.Dynamic;

namespace RPS_Modbus
{
    /// <summary>
    /// Configuration structure for OSI layers
    /// </summary>
    public class ModbusConfiguration
    {
        /// <summary>
        /// Serial port name
        /// </summary>
        public string PortName { get; set; } = "";

        /// <summary>
        /// Serial port baud rate
        /// </summary>
        public int BaudRate { get; set; } = 0;

        public int Timeout { get; set; }
        
        public bool IsServer { get; set; }

        public int ClientAddress { get; set; }

        public ModbusProtocolType ProtocolType = ModbusProtocolType.ASCII;
    }
}
