using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace rps_serialport_potentiometer
{
    /// <summary>
    /// Class <c>ApplicationLayer</c> for processing data content of messages
    /// </summary>
    public class ApplicationLayer
    {
        /// <summary>
        /// Link layer instance
        /// </summary>
        private readonly LinkLayer link;

        /// <summary>
        /// Timer used by server side for triggering value update
        /// </summary>
        private readonly Timer UpdateTimer = new Timer();

        /// <summary>
        /// Interval of server side value sync
        /// </summary>
        private readonly int UpdateInterval = 1000;

        /// <summary>
        /// Storage for registered addresses and their actual values
        /// </summary>
        private readonly Dictionary<char, int> value_tracker = new Dictionary<char, int>();

        /// <summary>
        /// Flag disallowing storage update during transmission
        /// </summary>
        private bool Locked { get; set; } = false;

        /// <summary>
        /// Flag marking if this instance is server or client
        /// </summary>
        public readonly bool IsServer = false;

        /// <summary>
        /// Flag marking connection status of instance
        /// </summary>
        public bool Connected { get { return link.Connected; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration structure</param>
        public ApplicationLayer(Configuration config)
        {
            IsServer = config.IsServer;
            UpdateInterval = config.AppUpdateInterval;
            // Init link layer
            link = new LinkLayer(config);
            if (IsServer)
            {
                // Init sync timer if instance is server
                UpdateTimer.Interval = UpdateInterval;
                UpdateTimer.Elapsed += Update_timer_Elapsed;
            }
            else
            {
                // Register event handler if instance is client
                link.OnMessageReceived += Link_OnMessageReceived;
            }
        }

        /// <summary>
        /// Handler for Elapsed event of sync timer
        /// </summary>
        /// <param name="sender">Timer instance that triggered this event</param>
        /// <param name="e">Event arguments</param>
        private void Update_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateTimer.Stop();
            Locked = true;
            // Iterate storage and sync all values
            foreach (char addr in value_tracker.Keys)
            {
                ApplicationPdu pdu = new ApplicationPdu
                {
                    Address = addr,
                    Value = value_tracker[addr]
                };
                ServerBroadcastValue(pdu);
            }
            Locked = false;
            UpdateTimer.Start();
            return;
        }

        /// <summary>
        /// Connect to bus
        /// </summary>
        /// <returns>true if success, false if error</returns>
        public bool Connect()
        {
            return link.Connect();
        }

        /// <summary>
        /// Disconnect from bus
        /// </summary>
        /// <returns>true if success, false if error</returns>
        public bool Disconnect()
        {
            return link.Disconnect();
        }

        /// <summary>
        /// Add address to storage - serves as message acceptance filter
        /// </summary>
        /// <param name="address">Address to be registered</param>
        /// <returns>true if success, false if error</returns>
        public bool RegisterAddress(char address)
        {
            if (value_tracker.Keys.Contains(address))
            {
                // Address already registered
                return false;
            }
            value_tracker.Add(address, 0);
            return true;
        }

        /// <summary>
        /// Start application server side service
        /// </summary>
        /// <returns>true if success, false if error</returns>
        public bool ServerStartService()
        {
            if (IsServer)
            {
                UpdateTimer.Start();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Stop application server side service
        /// </summary>
        /// <returns>true if success, false if error</returns>
        public bool ServerStopService()
        {
            if (IsServer)
            {
                UpdateTimer.Stop();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update value in storage
        /// </summary>
        /// <param name="address">Address of value</param>
        /// <param name="value">Value to be written</param>
        /// <returns>true if success, false if error</returns>
        public bool ServerUpdateValue(char address, int value)
        {
            if (value_tracker.ContainsKey(address) && !Locked)
            {
                value_tracker[address] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Send application protocol data unit to lower layer
        /// </summary>
        /// <param name="pdu">Application protocol data unit to send</param>
        /// <returns>true if success, false if error</returns>
        private bool ServerBroadcastValue(ApplicationPdu pdu)
        {
            if (IsServer)
            {
                try
                {
                    LinkMessage msg = new LinkMessage()
                    {
                        Data = pdu.Address + pdu.Value.ToString()
                    };
                    link.Send(msg);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Event handler for OnMessageReceived event of link
        /// </summary>
        /// <param name="sender">Link layer instance</param>
        /// <param name="msg">Event parameters</param>
        private void Link_OnMessageReceived(LinkLayer sender, LinkMessage msg)
        {
            if (!IsServer)
            {
                char addr = msg.Data[0];
                if (value_tracker.Keys.Contains(addr))
                {
                    ApplicationPdu pdu = new ApplicationPdu()
                    {
                        Address = addr,
                        Value = Convert.ToInt32(msg.Data.Substring(1))
                    };
                    value_tracker[pdu.Address] = pdu.Value;
                    Debug.WriteLine("APP-ADDR:" + pdu.Address + "-VAL:" + msg.Data.Substring(1));
                    TriggerApplicationUpdate(pdu);
                }
            }
        }

        /// <summary>
        /// Delegate for user notification about updated value
        /// </summary>
        /// <param name="sender">Application layer instance</param>
        /// <param name="pdu">Application protoocol data unit to send</param>
        public delegate void ApplicationUpdate(ApplicationLayer sender, ApplicationPdu pdu);

        /// <summary>
        /// Event for calling ApplicationUpdate delegate
        /// </summary>
        public event ApplicationUpdate OnApplicationUpdate;

        /// <summary>
        /// Method for triggering OnApplicationUpdate event
        /// </summary>
        /// <param name="pdu"></param>
        protected virtual void TriggerApplicationUpdate(ApplicationPdu pdu) { OnApplicationUpdate?.Invoke(this, pdu); }
    }
}
