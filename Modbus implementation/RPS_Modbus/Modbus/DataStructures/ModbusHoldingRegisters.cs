using System;
using System.Collections.Generic;
using System.IO;

namespace RPS_Modbus
{
    public class ModbusHoldingRegisters
    {
        // 0x0000 - 0xffff
        private Dictionary<ushort, ushort> Storage = new Dictionary<ushort, ushort>();

        public void Write(ushort address, ushort value)
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

        public ushort Read(ushort address)
        {
            try
            {
                return (Storage[address]);
            }
            catch
            {
                throw new InvalidDataException("ILLEGAL_DATA_ADDRESS");
            }
        }
    }
}
