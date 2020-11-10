using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace RPS_Modbus
{
    /// <summary>
    /// Link layer of RPS protocol
    /// </summary>
    public class ModbusLinkAscii : IModbusLink
    {
        /// <summary>
        /// Message processing state machine states
        /// </summary>
        private enum ModbusAsciiLinkState
        {
            IDLE,
            RECEIVING,
        }

        /// <summary>
        /// Actual state of link
        /// </summary>
        private ModbusAsciiLinkState ActualState = ModbusAsciiLinkState.IDLE;

        /// <summary>
        /// Physical layer abstraction instance
        /// </summary>
        private readonly ModbusSerialPort PHY;

        /// <summary>
        /// Buffer for storing received bytes
        /// </summary>
        private readonly byte[] ActualFrame = new byte[512];

        /// <summary>
        /// Actual index in reception buffer
        /// </summary>
        private int ActualFrameIndex;

        /// <summary>
        /// Id of actual message being processed - used for timeout handling
        /// </summary>
        private uint ActualMessageId = 0;

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
        private readonly Thread LinkThread;

        /// <summary>
        /// Physical layer connection status
        /// </summary>
        public bool Connected { get { return PHY != null && PHY.Connected; } set { return; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration structure</param>
        public ModbusLinkAscii(ModbusConfiguration config)
        {
            // Init serial port driver
            PHY = new ModbusSerialPort(config);
            // Init link layer thread
            LinkThread = new Thread(ModbusAsciiLinkTask);
            // Configure timeout timer
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
            if (PHY.Connect())
            {
                // Start the link thread if connection was successful
                ActualState = ModbusAsciiLinkState.IDLE;
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
            // Stop link thread            
            LinkThread.Abort();
            ActualState = ModbusAsciiLinkState.IDLE;
            return PHY.Disconnect();
        }

        /// <summary>
        /// Convert message payload data ASCII format
        /// </summary>
        /// <param name="data">Buffer containing message payload in numeric format</param>
        /// <returns>Buffer converted to ASCII format</returns>        
        private byte[] ConvertDataToAscii(byte[] data)
        {
            // Calculate new size and alocate result buffer
            byte[] newData = new byte[data.Length * 2];
            int index = 0;
            // Iterate input buffer, convert values to ASCII and store them in output buffer
            foreach (byte val in data)
            {
                newData[index++] = Convert.ToByte((val & 0xf0) >> 4).ToAscii();
                newData[index++] = Convert.ToByte(val & 0x0f).ToAscii();
            }
            return newData;
        }

        /// <summary>
        /// Build data frame and transmit it to bus
        /// </summary>
        /// <param name="msg">Message payload to be transmitted</param>
        /// <returns>true if success, false if error</returns>
        public bool Send(byte[] msg)
        {
            // Allocate temporary buffer for payload + LRC
            byte[] tmpData = new byte[msg.Length + 1];
            // Copy message to temporary buffer
            Array.Copy(msg, tmpData, msg.Length);
            // Calculate LRC and store it to temporary buffer
            tmpData[tmpData.Length - 1] = msg.CalculateLrc(msg.Length);
            // Calculate transmitted byte buffer size and allocate it
            int txLen = (tmpData.Length * 2) + 3;
            byte[] txData = new byte[txLen];
            // Convert payload and LRC to ASCII format
            tmpData = ConvertDataToAscii(tmpData);
            // Add start character to transmit buffer
            txData[0] = Convert.ToByte(':');
            // Copy data in ASCII format to transmit buffer
            Array.Copy(tmpData, 0, txData, 1, tmpData.Length);
            // Add trailing CRLF sequence to transmit buffer
            txData[txData.Length - 2] = Convert.ToByte('\r');
            txData[txData.Length - 1] = Convert.ToByte('\n');
            // Send finished data frame
            Debug.WriteLine("LINK - TX_MSG: " + BitConverter.ToString(msg));
            return PHY.Transmit(txData);
        }

        /// <summary>
        /// Process received data bytes and notify upper layer if data is valid
        /// </summary>
        private void ProcessReceivedData()
        {
            // Get payload length in ASCII format
            int rxLen = ActualFrameIndex - 2;
            if ((rxLen % 2) > 0)
            {
                // Payload length is odd, data is invalid
                Debug.WriteLine("LINK - DATA_FORMAT_ERROR");
                return;
            }
            // Get numeric data length and alocate buffer
            rxLen /= 2;
            byte[] rxData = new byte[rxLen];
            // Iterate actual frame buffer and convert data to numeric format
            for (int i = 0; i < rxLen * 2; i += 2)
            {
                int val = 0;
                val |= (ActualFrame[i].FromAscii() << 4);
                val |= ActualFrame[i + 1].FromAscii();
                rxData[i / 2] = Convert.ToByte(val);
            }
            // Get LRC from received frame
            int rxLrc = Convert.ToByte((ActualFrame[ActualFrameIndex - 2].FromAscii() << 4) | (ActualFrame[ActualFrameIndex - 1].FromAscii()));
            // Calculate LRC again from received payload
            byte tmpLrc = rxData.CalculateLrc(rxData.Length);
            if (rxLrc != tmpLrc)
            {
                // LRC values differ, data is invalid
                Debug.WriteLine("LINK - LRC_ERROR");
                return;
            }
            // LRC check successful, notify upper layer
            Debug.WriteLine("LINK - RX_MSG: " + BitConverter.ToString(rxData));
            TriggerMessageReceived(rxData);
        }

        /// <summary>
        /// Task for processing received data running in thread
        /// </summary>
        private void ModbusAsciiLinkTask()
        {
            while (true)
            {
                // Pop byte from receive ring buffer
                Thread.Sleep(0);
                int value = PHY.Receive();
                if (value != -1)
                {
                    byte ReceivedByte = Convert.ToByte(value);
                    switch (ActualState)
                    {
                        // Link is idle, wait for start character
                        case ModbusAsciiLinkState.IDLE:
                            if (ReceivedByte == ':')
                            {
                                // Start character received, start timeout timer and set state to receiving
                                TkickMessageId = ActualMessageId;
                                TimeoutCounter.Start();
                                // Reset reception buffer index
                                ActualFrameIndex = 0;
                                ActualState = ModbusAsciiLinkState.RECEIVING;
                            }
                            break;
                        case ModbusAsciiLinkState.RECEIVING:
                            // Link is receiving a message
                            if (ReceivedByte == '\n')
                            {
                                // Stop character received, stop timeout timer
                                TimeoutCounter.Stop();
                                // Check data integrity and notify upper layer
                                ProcessReceivedData();
                                // Set state to idle
                                ActualState = ModbusAsciiLinkState.IDLE;
                                // Increment message id
                                ActualMessageId++;
                                break;
                            }
                            if (ReceivedByte == '\r')
                            {
                                // do not store CR character
                                break;
                            }
                            if (ActualFrameIndex < ActualFrame.Length)
                            {
                                // Write received byte buffer
                                ActualFrame[ActualFrameIndex++] = ReceivedByte;
                            }
                            // If buffer overflow occured, skip and wait for timeout
                            break;
                        default:
                            break;
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
            if (Connected)
            {
                if (ActualState == ModbusAsciiLinkState.IDLE)
                {
                    // Timeout occured but bus is idle (message processed) should not happen
                    TimeoutCounter.Stop();
                    Debug.WriteLine("LINK - IDLE_TIMEOUT");
                    return;
                }
                if (ActualMessageId != TkickMessageId)
                {
                    // Timeout occured but link is already handling another message - should not happen
                    TimeoutCounter.Stop();
                    Debug.WriteLine("LINK - BUSY_TIMEOUT");
                    return;
                }
                // Timeout occured and link is stuck, reset (required transport layer for handling possible data loss)
                Debug.WriteLine("LINK - STUCK_TIMEOUT");
                ActualFrameIndex = 0;
                ActualMessageId = 0;
                TkickMessageId = 0;
                TriggerTimeoutOccured();
                ActualState = ModbusAsciiLinkState.IDLE;
                return;
            }
        }

        #region TIMEOUT_NOTIFICATION_EVENT
        public event EventHandler OnTimeoutOccured;
        protected virtual void TriggerTimeoutOccured() { OnTimeoutOccured?.Invoke(this, new EventArgs()); }
        #endregion

        #region MESSAGE_RECEIVED_EVENT
        public event EventHandler<MessageReceivedArgs> OnMessageReceived;
        protected virtual void TriggerMessageReceived(byte[] msg) { OnMessageReceived?.Invoke(this, new MessageReceivedArgs(msg)); }
        #endregion
    }
}
