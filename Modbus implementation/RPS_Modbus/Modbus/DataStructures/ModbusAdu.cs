using System;

namespace RPS_Modbus
{
    /// <summary>
    /// Class <c>Message</c> for better data interpretation and processing
    /// </summary>
    public class ModbusAdu
    {
        public byte Address;

        public byte Function;

        public byte[] Data;

        public override string ToString()
        {
            return "ADDR:" + Address.ToString("X") + "-CMD:" + Function.ToString("X") + "-DATA:" + BitConverter.ToString(Data);
        }
    }
}
