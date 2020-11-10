using System;

namespace RPS_Modbus
{
    public static class ModbusConversion
    {
        public static int FromAscii(this byte value)
        {
            try
            {
                string ch = Convert.ToChar(value).ToString();
                return Convert.ToInt32(ch, 16);
            }
            catch
            {
                return 0;
                //throw new InvalidDataException("Invalid input value.");
            }
        }

        public static byte ToAscii(this byte value)
        {
            return Convert.ToByte(Convert.ToChar(value.ToString("X")));
        }
    }
}
