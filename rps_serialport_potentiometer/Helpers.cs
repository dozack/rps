using System;
using System.Windows.Forms;

namespace rps_serialport_potentiometer
{
    public static class Helpers
    {
        public static void Clear(this byte[] array)
        {
            Array.Clear(array, 0, array.Length);
        }

        public static void Copy(this byte [] array, byte [] destination)
        {
            Array.Copy(array, destination, array.Length);
        }
    }
}
