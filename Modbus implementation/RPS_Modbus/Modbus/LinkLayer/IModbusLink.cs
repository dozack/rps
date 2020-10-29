using System;

namespace RPS_Modbus
{
    /// <summary>
    /// Interface for implementation of different link layer types for modbus protocol
    /// </summary>
    public interface IModbusLink
    {
        /// <summary>
        /// Connection status flag
        /// </summary>
        bool Connected { get; set; }

        /// <summary>
        /// Connect to physical layer medium
        /// </summary>
        /// <returns>True on success</returns>
        bool Connect();

        /// <summary>
        /// Disconnect from physical layer medium
        /// </summary>
        /// <returns>True on success</returns>
        bool Disconnect();

        /// <summary>
        /// Transmit modbus message payload
        /// </summary>
        /// <param name="msg">Message payload in numeric format (ADDRESS + COMMAND + DATA)</param>
        /// <returns>True on success</returns>
        bool Send(byte[] msg);

        /// <summary>
        /// Event for message reception success notification
        /// </summary>
        event EventHandler<MessageReceivedArgs> OnMessageReceived;

        /// <summary>
        /// Event for message reception timeout notification - unused in RTU
        /// </summary>
        event EventHandler OnTimeoutOccured;
    }

    /// <summary>
    /// <c>EventArgs</c> extension for modbus link layer interface event
    /// </summary>
    public class MessageReceivedArgs : EventArgs
    {
        public byte[] Message { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msg">Received message payload in numeric format</param>
        public MessageReceivedArgs(byte[] msg)
        {
            Message = msg;
        }
    }
}
