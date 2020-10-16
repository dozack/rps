namespace rps_serialport_potentiometer
{
    /// <summary>
    /// Class <c>Message</c> for better data interpretation and processing
    /// </summary>
    public class LinkMessage
    {
        /// <summary>
        /// Message length in bytes (including command byte)
        /// </summary>
        public uint Length;

        /// <summary>
        ///Data converted to string
        /// </summary>
        public string Data;

        /// <summary>
        /// Convert whole structure to single string
        /// </summary>
        /// <returns></returns>
        public string Raw()
        {
            return Length.ToString() + Data;
        }

        /// <summary>
        /// Override ToString method for better logging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "MSG-LEN:" + Length.ToString() + "-DATA:" + Data;
        }
    }
}
