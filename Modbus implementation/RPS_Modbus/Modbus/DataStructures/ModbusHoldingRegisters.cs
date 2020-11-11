using System;
using System.Collections.Generic;

namespace RPS_Modbus
{
    public class ModbusHoldingRegisters
    {
        private Dictionary<ushort, ushort> Storage = new Dictionary<ushort, ushort>();

        public void Write(ushort address, ushort value, bool notify)
        {
            try
            {
                Storage[address] = value;
                if (notify)
                {
                    TriggerValueUpdated(address, value);
                }
            }
            catch
            {
                throw new ArgumentException("ILLEGAL_DATA_ADDRESS || ILLEGAL_DATA_VALUE");
            }
        }

        public ushort Read(ushort address)
        {
            try
            {
                return Storage[address];
            }
            catch
            {
                throw new ArgumentException("ILLEGAL_DATA_ADDRESS");
            }
        }

        public delegate void ValueUpdated(ModbusHoldingRegisters sender, ushort address, ushort value);

        public event ValueUpdated OnValueUpdated;

        protected virtual void TriggerValueUpdated(ushort address, ushort value) { OnValueUpdated?.Invoke(this, address, value); }
    }
}
