using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace RPS_Modbus
{
    /// <summary>
    /// Link layer of RPS protocol
    /// </summary>
    public class ModbusAsciiLink
    {
        /// <summary>
        /// Message processing state machine states
        /// </summary>
        private enum ModbusAsciiLinkState
        {
            IDLE,
            GET_ADDR1,
            GET_ADDR0,
            GET_FUNC1,
            GET_FUNC0,
            GET_DATA1,
            GET_DATA0
        }

        /// <summary>
        /// Actual state of link
        /// </summary>
        private ModbusAsciiLinkState ActualState = ModbusAsciiLinkState.IDLE;

        /// <summary>
        /// Physical layer abstraction instance
        /// </summary>
        private readonly ModbusSerialStream phy;

        /// <summary>
        /// Actual processed message
        /// </summary>
        private ModbusAdu ActualMessage = new ModbusAdu();

        /// <summary>
        /// Id of actual message being processed - used for timeout handling
        /// </summary>
        private uint ActualMessageId = 0;

        private byte[] DataStorage = new byte[512];

        private uint DataCounter = 0;

        /// <summary>
        /// Id of message that was being processed in time of timer start - used for timeout handling 
        /// </summary>
        private uint TkickMessageId = 0;

        /// <summary>
        /// Timer instance for timeout implementation
        /// </summary>
        private readonly System.Timers.Timer TimeoutCounter = new System.Timers.Timer();

        /// <summary>
        /// Link layer thread instance
        /// </summary>
        private Thread LinkThread;

        /// <summary>
        /// Physical layer connection status
        /// </summary>
        public bool Connected { get { return phy != null && phy.Connected; } set { return; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration structure</param>
        public ModbusAsciiLink(Configuration config)
        {
            phy = new ModbusSerialStream(config);
            LinkThread = new Thread(ModbusAsciiLinkTask);
            ActualState = ModbusAsciiLinkState.IDLE;
            TimeoutCounter.Interval = 1000;
            TimeoutCounter.AutoReset = false;
            TimeoutCounter.Elapsed += TimeoutHandler;
        }

        /// <summary>
        /// Open connection and access bus
        /// </summary>
        /// <returns>true if success, false if error</returns>
        public bool Connect()
        {
            if (phy.Connect() == true)
            {
                LinkThread.Start();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Close bus connection
        /// </summary>
        /// <returns>true if success, false if error</returns>
        public bool Disconnect()
        {
            LinkThread.Abort();
            return phy.Disconnect();
        }

        /// <summary>
        /// Build packet and transmit it on bus
        /// </summary>
        /// <param name="msg">Message to be transmitted</param>
        /// <returns>true if success, false if error</returns>
        public bool Send(ModbusAdu msg)
        {
            int len = msg.Data.Length;
            if (len > 9) { len = 9; }
            string raw = '#' + len.ToString() + msg.Data;
            byte[] data = Encoding.ASCII.GetBytes(raw);
            if (phy.Transmit(data) == true)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Task for processing received data running in thread
        /// </summary>
        private void ModbusAsciiLinkTask()
        {
            while (true)
            {
                bool MessageReceived = false;
                // Pop byte from receive ring buffer
                int value = phy.Receive();
                if (value != -1)
                {
                    byte ReceivedByte = Convert.ToByte(value);
                    switch (ActualState)
                    {
                        // Bus is idle and waiting for start character
                        case ModbusAsciiLinkState.IDLE:
                            if (ReceivedByte == ':')
                            {
                                // Save actual message id and start timeout timer
                                TkickMessageId = ActualMessageId;
                                DataCounter = 0;
                                TimeoutCounter.Start();
                                ActualState = ModbusAsciiLinkState.GET_ADDR1;
                            }
                            break;
                        // Receiving device address
                        case ModbusAsciiLinkState.GET_ADDR1:
                            ActualMessage = new ModbusAdu()
                            {
                                Address = ReceivedByte.HighByteFromAscii(),
                                Data = new byte[512]
                            };
                            ActualState = ModbusAsciiLinkState.GET_ADDR0;
                            break;
                        case ModbusAsciiLinkState.GET_ADDR0:
                            ActualMessage.Address |= ReceivedByte.LowByteFromAscii();
                            ActualState = ModbusAsciiLinkState.GET_FUNC1;
                            break;
                        case ModbusAsciiLinkState.GET_FUNC1:
                            ActualMessage.Function = ReceivedByte.HighByteFromAscii();
                            ActualState = ModbusAsciiLinkState.GET_FUNC0;
                            break;
                        case ModbusAsciiLinkState.GET_FUNC0:
                            ActualMessage.Function |= ReceivedByte.LowByteFromAscii();
                            ActualState = ModbusAsciiLinkState.GET_DATA1;
                            break;
                        // Receiving message data
                        case ModbusAsciiLinkState.GET_DATA1:
                            // Ignore carriage return character 
                            if (ReceivedByte == '\r')
                            {
                                break;
                            }
                            // End of message detected
                            if (ReceivedByte == '\n')
                            {
                                MessageReceived = true;
                                break;
                            }
                            // Store received byte to data
                            DataStorage[DataCounter] = ReceivedByte.HighByteFromAscii();
                            ActualState = ModbusAsciiLinkState.GET_DATA0;
                            break;
                        case ModbusAsciiLinkState.GET_DATA0:
                            // Ignore carriage return character 
                            if (ReceivedByte == '\r')
                            {
                                break;
                            }
                            // End of message detected
                            if (ReceivedByte == '\n')
                            {
                                MessageReceived = true;
                                break;
                            }
                            DataStorage[DataCounter++] |= ReceivedByte.LowByteFromAscii();
                            ActualState = ModbusAsciiLinkState.GET_DATA1;
                            break;
                        // Should not get there
                        default:
                            break;
                    }
                    if (MessageReceived)
                    {
                        ActualMessage.Data = new byte[DataCounter];
                        Array.Copy(DataStorage, ActualMessage.Data, DataCounter);

                        ActualMessage.DataLength = Convert.ToByte(DataCounter);
                        // Message length achieved, assign id to next message
                        ActualMessageId++;
                        // Stop timeout timer
                        TimeoutCounter.Stop();
                        // Log received data to output window
                        Debug.WriteLine("LINK-" + ActualMessage.ToString());
                        // Trigger OnMessageReceived event
                        TriggerMessageReceived(ActualMessage);
                        // Return link status back to idle state so next message can be processed
                        ActualState = ModbusAsciiLinkState.IDLE;
                    }
                }
            }
        }

        /// <summary>
        /// Handler for timers Elapsed event
        /// </summary>
        /// <param name="sender">Timer instance that triggered event</param>
        /// <param name="e">Arguments</param>
        private void TimeoutHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ActualState == ModbusAsciiLinkState.IDLE)
            {
                // Timeout occured but bus is idle (message processed) should not happen
                Debug.WriteLine("LINK-IDLE_TIMEOUT");
                return;
            }
            if (ActualMessageId != TkickMessageId)
            {
                // Timeout occured but link is already handling another message - should not happen
                Debug.WriteLine("LINK-BUSY_TIMEOUT");
                return;
            }
            // Timeout occured and link is stuck, reset (required transport layer for handling possible data loss)
            Debug.WriteLine("LINK-STUCK_TIMEOUT");
            ActualMessage = new ModbusAdu();
            ActualMessageId = 0;
            TkickMessageId = 0;
            ActualState = ModbusAsciiLinkState.IDLE;
            return;
        }

        /// <summary>
        /// Delegate for sharing received messages with higher layers
        /// </summary>
        /// <param name="sender">Instance of link layer</param>
        /// <param name="msg">Received message</param>
        public delegate void MessageReceived(ModbusAsciiLink sender, ModbusAdu msg);

        /// <summary>
        /// Event for calling MessageReceived delegate
        /// </summary>
        public event MessageReceived OnMessageReceived;

        /// <summary>
        /// Method for triggering OnMessageReceived event
        /// </summary>
        /// <param name="msg">Received message</param>
        protected virtual void TriggerMessageReceived(ModbusAdu msg) { OnMessageReceived?.Invoke(this, msg); }
    }
}
