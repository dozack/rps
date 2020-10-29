using External.MicroTimer;
using System;
using System.Diagnostics;
using System.Threading;

namespace RPS_Modbus.Modbus.LinkLayer
{
    public class ModbusLinkRtu : IModbusLink
    {
        /// <summary>
        /// Message processing state machine states
        /// </summary>
        private enum ModbusRtuLinkState
        {
            INIT,
            IDLE,
            RECEIVING,
            PROCESSING,
        }

        /// <summary>
        /// Actual state of link
        /// </summary>
        private ModbusRtuLinkState ActualState = ModbusRtuLinkState.INIT;

        /// <summary>
        /// Physical layer abstraction instance
        /// </summary>
        private readonly ModbusSerialPort PHY;

        private readonly MicroTimer FramingTimer = new MicroTimer();

        /// <summary>
        /// Buffer for storing received bytes
        /// </summary>
        private readonly byte[] ActualFrame = new byte[256];

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
        public ModbusLinkRtu(Configuration config)
        {
            // Init serial port driver
            PHY = new ModbusSerialPort(config);
            // Init link layer thread
            LinkThread = new Thread(ModbusAsciiLinkTask);
            // Configure timeout timer
            TimeoutCounter.Interval = 1000;
            TimeoutCounter.AutoReset = false;
            TimeoutCounter.Elapsed += TimeoutHandler;
            ConfigureTiming(config.BaudRate / 4);
            FramingTimer.OnTimeout += FramingTimeoutHandler;
        }

        private void ConfigureTiming(int baudrate)
        {
            // source: https://stackoverflow.com/questions/20740012/calculating-modbus-rtu-3-5-character-time
            // Modbus states that a baud rate higher than 19200 must use a fixed 750 us 
            // for inter character time out and 1.75 ms for a frame delay.
            // For baud rates below 19200 the timeing is more critical and has to be calculated.
            // E.g. 9600 baud in a 10 bit packet is 960 characters per second
            // In milliseconds this will be 960characters per 1000ms. So for 1 character
            // 1000ms/960characters is 1.04167ms per character and finaly modbus states an
            // intercharacter must be 1.5T or 1.5 times longer than a normal character and thus
            // 1.5T = 1.04167ms * 1.5 = 1.5625ms. A frame delay is 3.5T.

            if (baudrate > 19200)
            {
                FramingTimer.MicroSeconds = 1750;
            }
            else
            {
                FramingTimer.MicroSeconds = 35000000 / baudrate;
            }
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
                LinkThread.Start();
                FramingTimer.Start();
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
            FramingTimer.Stop();
            // Stop link thread
            LinkThread.Abort();
            return PHY.Disconnect();
        }

        /// <summary>
        /// Build data frame and transmit it to bus
        /// </summary>
        /// <param name="msg">Message payload to be transmitted</param>
        /// <returns>true if success, false if error</returns>
        public bool Send(byte[] msg)
        {
            return false;
        }

        /// <summary>
        /// Process received data bytes and notify upper layer if data is valid
        /// </summary>
        private void ProcessReceivedData()
        {
            byte[] rxData = new byte[ActualFrameIndex];
            Array.Copy(ActualFrame, 0, rxData, 0, ActualFrameIndex);
            Debug.WriteLine("LINK - RECV_MSG: " + BitConverter.ToString(rxData));
            return;
        }

        /// <summary>
        /// Task for processing received data running in thread
        /// </summary>
        private void ModbusAsciiLinkTask()
        {
            while (true)
            { 
                Thread.Sleep(0);
                int value;
                switch (ActualState)
                {
                    case ModbusRtuLinkState.INIT:
                        // Init mode, wait for first 3.5 char framing timeout
                        break;
                    case ModbusRtuLinkState.IDLE:
                        // Idle mode, wait for incoming message
                        value = PHY.Receive();
                        if (value != -1)
                        {
                            byte ReceivedByte = Convert.ToByte(value);
                            TkickMessageId = ActualMessageId;
                            ActualFrameIndex = 0;
                            ActualFrame[ActualFrameIndex++] = ReceivedByte;
                            ActualState = ModbusRtuLinkState.RECEIVING;
                            FramingTimer.Start();
                        }
                        break;
                    case ModbusRtuLinkState.RECEIVING:
                        // Receive all message bytes until framing timeout
                        value = PHY.Receive();
                        if (value != -1)
                        {
                            FramingTimer.Stop();
                            byte ReceivedByte = Convert.ToByte(value);
                            ActualFrame[ActualFrameIndex++] = ReceivedByte;
                            FramingTimer.Start();
                        }
                        break;
                    case ModbusRtuLinkState.PROCESSING:
                        // Process message
                        ProcessReceivedData();
                        ActualMessageId++;
                        ActualState = ModbusRtuLinkState.IDLE;
                        break;
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
            return;
        }

        /// <summary>
        /// Handler for 3.5 character framing timer - should retun as fast as possible
        /// </summary>
        private void FramingTimeoutHandler(object sender)
        {
            switch (ActualState)
            {
                case ModbusRtuLinkState.INIT:
                    // Init state timeout elapsed, set state to idle
                    Debug.WriteLine("LINK: SYNC_OK");
                    ActualState = ModbusRtuLinkState.IDLE;
                    break;
                case ModbusRtuLinkState.RECEIVING:
                    // Message reception timed out, process received data
                    ActualState = ModbusRtuLinkState.PROCESSING;
                    break;
                case ModbusRtuLinkState.IDLE:
                case ModbusRtuLinkState.PROCESSING:
                    // Link is idle or processing message, wait
                    //Debug.WriteLine("LINK: IDLE/PROCESSING -> WAIT...");
                    break;
            }
            return;
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

