using System;
using System.Collections.Generic;
using System.IO;

namespace RPS_Modbus
{
    public class ModbusCoils
    {
        private readonly Dictionary<ushort, bool> Storage = new Dictionary<ushort, bool>();

        public void Write(ushort address, bool value)
        {
            try
            {
                Storage[address] = value;
            }
            catch
            {
                throw new InvalidDataException("ILLEGAL_DATA_VALUE");
            }
        }

        public byte Read(ushort address)
        {
            try
            {
                return Convert.ToByte(Storage[address]);
            }
            catch
            {
                throw new InvalidDataException("ILLEGAL_DATA_ADDRESS");
            }
        }
    }
}