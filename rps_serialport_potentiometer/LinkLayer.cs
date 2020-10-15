using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace rps_serialport_potentiometer
{
    /// <summary>
    /// Link layer of RPS protocol
    /// </summary>
    public class LinkLayer
    {
        /// <summary>
        /// Message processing state machine states
        /// </summary>
        private enum LinkStatus
        {
            IDLE,
            GET_LEN,
            GET_DATA
        }

        /// <summary>
        /// Actual state of link
        /// </summary>
        private LinkStatus status = LinkStatus.IDLE;

        /// <summary>
        /// Physical layer abstraction instance
        /// </summary>
        private readonly PhysicalLayer phy;

        /// <summary>
        /// Actual processed message
        /// </summary>
        private LinkMessage actl_message = new LinkMessage();

        /// <summary>
        /// Counter of received bytes for actual message
        /// </summary>
        private int rx_counter = 0;

        /// <summary>
        /// Id of actual message being processed - used for timeout handling
        /// </summary>
        private uint actl_message_id = 0;

        /// <summary>
        /// Id of message that was being processed in time of timer start - used for timeout handling 
        /// </summary>
        private uint tkick_message_id = 0;

        /// <summary>
        /// Timer instance for timeout implementation
        /// </summary>
        private readonly System.Timers.Timer timer = new System.Timers.Timer();

        /// <summary>
        /// Link layer thread instance
        /// </summary>
        private Thread link_thread;

        /// <summary>
        /// Physical layer connection status
        /// </summary>
        public bool Connected { get { return phy != null && phy.Connected; } set { return; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration structure</param>
        public LinkLayer(Configuration config)
        {
            phy = new PhysicalLayer(config);
            link_thread = new Thread(LinkThreadTask);
            status = LinkStatus.IDLE;
            link_thread.Start();
            timer.Interval = 1000;
            timer.AutoReset = false;
            timer.Elapsed += TimeoutHandler;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~LinkLayer()
        {
            link_thread.Abort();
            Disconnect();
        }

        /// <summary>
        /// Open connection and access bus
        /// </summary>
        /// <returns>true if success, false if bus is already connected, null if error</returns>
        public bool? Connect()
        {
            return phy.Connect();
        }

        /// <summary>
        /// Close bus connection
        /// </summary>
        /// <returns>true if success, false if bus is already disconnected, null if error</returns>
        public bool? Disconnect()
        {
            return phy.Disconnect();
        }

        /// <summary>
        /// Build packet and transmit it on bus
        /// </summary>
        /// <param name="msg">Message to be transmitted</param>
        /// <returns>>true if success, false if bus is not available, null if error</returns>
        public bool? Send(LinkMessage msg)
        {
            string raw = '#' + msg.Raw();
            byte[] data = Encoding.ASCII.GetBytes(raw);
            return phy.Transmit(data);
        }

        /// <summary>
        /// Task for processing received data running in thread
        /// </summary>
        private void LinkThreadTask()
        {
            while (true)
            {
                // Sleep delay
                Thread.Sleep(1);
                // Pop byte from receive ring buffer
                int value = phy.Receive();
                if (value != -1)
                {
                    char rx = Convert.ToChar(value);
                    switch (status)
                    {
                        // Bus is idle and waiting for start character
                        case LinkStatus.IDLE:
                            if (rx == '#')
                            {
                                // Save actual message id and start timeout timer
                                tkick_message_id = actl_message_id;
                                timer.Start();
                                // Proceed to length reception
                                status = LinkStatus.GET_LEN;
                            }
                            break;
                        // Receiving message length
                        case LinkStatus.GET_LEN:
                            uint length = (uint)(rx - '0');
                            actl_message = new LinkMessage()
                            {
                                Length = length
                            };
                            // Reset data counter
                            rx_counter = 0;
                            // Set link status to data reception
                            status = LinkStatus.GET_DATA;
                            break;
                        // Receiving message data
                        case LinkStatus.GET_DATA:
                            // Save data and increment counter
                            actl_message.Data += rx;
                            rx_counter++;
                            if (rx_counter == actl_message.Length)
                            {
                                // Message length achieved, assign id to next message
                                actl_message_id++;
                                // Stop timeout timer
                                timer.Stop();
                                // Log received data to output window
                                Debug.WriteLine(actl_message.ToString());
                                // Trigger OnMessageReceived event
                                TriggerMessageReceived(actl_message);
                                // Return link status back to idle state so next message can be processed
                                status = LinkStatus.IDLE;
                            }
                            break;
                        // Should not get there
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
            if (status == LinkStatus.IDLE)
            {
                // Timeout occured but bus is idle (message processed) should not happen
                Debug.WriteLine("Timeout - BUS IDLE");
                return;
            }
            if (actl_message_id != tkick_message_id)
            {
                // Timeout occured but link is already handling another message - should not happen
                Debug.WriteLine("Timeout - LINK BUSY");
                return;
            }
            // Timeout occured and link is stuck, reset (required transport layer for handling possible data loss)
            Debug.WriteLine("Timeout - LINK STUCK");
            actl_message = new LinkMessage();
            rx_counter = 0;
            actl_message_id = 0;
            tkick_message_id = 0;
            status = LinkStatus.IDLE;
            return;
        }

        /// <summary>
        /// Delegate for sharing received messages with higher layers
        /// </summary>
        /// <param name="sender">Instance of link layer</param>
        /// <param name="msg">Received message</param>
        public delegate void MessageReceived(LinkLayer sender, LinkMessage msg);

        /// <summary>
        /// Event for calling MessageReceived delegate
        /// </summary>
        public event MessageReceived OnMessageReceived;

        /// <summary>
        /// Method for triggering OnMessageReceived event
        /// </summary>
        /// <param name="msg">Received message</param>
        protected virtual void TriggerMessageReceived(LinkMessage msg) { OnMessageReceived?.Invoke(this, msg); }
    }
}
