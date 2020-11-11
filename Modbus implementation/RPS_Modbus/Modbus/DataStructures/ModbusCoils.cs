using System;
using System.Collections.Generic;

namespace RPS_Modbus
{
    public class ModbusCoils
    {
        private readonly Dictionary<ushort, bool> Storage = new Dictionary<ushort, bool>();

        public void Write(ushort address, bool value, bool notify)
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

        public byte Read(ushort address)
        {
            try
            {
                return Convert.ToByte(Storage[address]);
            }
            catch
            {
                throw new ArgumentException("ILLEGAL_DATA_ADDRESS");
            }
        }

        public delegate void ValueUpdated(ModbusCoils sender, ushort address, bool value);

        public event ValueUpdated OnValueUpdated;

        protected virtual void TriggerValueUpdated(ushort address, bool value) { OnValueUpdated?.Invoke(this, address, value); }
    }
}