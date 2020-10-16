namespace rps_serialport_potentiometer
{
    /// <summary>
    /// Data structure for aplication data unit
    /// </summary>
    public class ApplicationPdu
    {
        /// <summary>
        /// Address of value or register for update
        /// </summary>
        public char Address;

        /// <summary>
        /// New value to be written
        /// </summary>
        public int Value;
    }
}
