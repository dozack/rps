using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;
using System.Drawing;

namespace rps_serialport_potentiometer
{
    public partial class SerialPortWindow : Form
    {
        /// <summary>
        /// Protoocol application layer instance
        /// </summary>
        ApplicationLayer ProtocolHandler;

        /// <summary>
        /// Storage for supported baud rates
        /// </summary>
        private Dictionary<string, int> baudrates = new Dictionary<string, int>()
        {
            {"9600", 9600 },
            {"19200", 57600},
            {"115200", 115200},
            {"921600", 921600 }
        };

        /// <summary>
        /// Constructor
        /// </summary>
        public SerialPortWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler for Click event of bttnConnection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttnConnection_Click(object sender, EventArgs e)
        {
            if (ProtocolHandler == null || !ProtocolHandler.Connected)
            {
                // ProtocolHandler not initalized or disconnected
                Configuration config = new Configuration()
                {
                    PortName = cmbPorts.SelectedItem.ToString(),
                    BaudRate = baudrates[cmbBaud.SelectedItem.ToString()],
                    IsServer = chkMaster.Checked,
                };
                // Init new instance of application layer
                ProtocolHandler = new ApplicationLayer(config);

                // Try to connect to bus
                if (!ProtocolHandler.Connect())
                {
                    MessageBox.Show("Error during connection, try again.");
                    return;
                }

                // If app is client, register handler for notifications
                if (!ProtocolHandler.IsServer)
                {
                    ProtocolHandler.OnApplicationUpdate += ProtocolHandler_OnApplicationUpdate;
                }
                
                // Register trackbar address
                ProtocolHandler.RegisterAddress('A');
                // Register button address
                ProtocolHandler.RegisterAddress('D');

                if (ProtocolHandler.IsServer)
                {
                    // Start service if instance is server
                    ProtocolHandler.ServerStartService();
                }

                // Update controls
                bttnConnection.Text = "Disconnect";
                grpData.Enabled = true;
                cmbPorts.Enabled = false;
                cmbBaud.Enabled = false;
                chkMaster.Enabled = false;
            }
            else
            {
                // Disconnect from bus
                ProtocolHandler.Disconnect();

                // Update controls
                bttnConnection.Text = "Connect";
                grpData.Enabled = false;
                cmbPorts.Enabled = true;
                cmbBaud.Enabled = true;
                chkMaster.Enabled = true;
            }
        }

        /// <summary>
        /// Handler for application layer notifications
        /// </summary>
        /// <param name="sender"><c>ApplicationLayer</c> instance</param>
        /// <param name="pdu">Application protocol data unit</param>
        private void ProtocolHandler_OnApplicationUpdate(ApplicationLayer sender, ApplicationPdu pdu)
        {
            switch (pdu.Address)
            {
                case 'A':
                    // Update trackbar value
                    if (pdu.Value <= trckAnalog.Maximum)
                    {
                        Invoke(new Action(() =>
                        {
                            trckAnalog.Value = pdu.Value;
                        }));
                    }
                    break;
                case 'D':
                    // Update button color
                    bool value = Convert.ToBoolean(pdu.Value);
                    if (value)
                    {
                        Invoke(new Action(() =>
                        {
                            bttnDigitial.BackColor = Color.Green;
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            bttnDigitial.BackColor = Color.Gray;
                        }));
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handler of Load event for initialization of controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortWindow_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmbPorts.Items.Add(port);
            }
            if (cmbPorts.Items.Count > 0)
            {
                cmbPorts.SelectedIndex = 0;
            }
            foreach (KeyValuePair<string, int> baud in baudrates)
            {
                cmbBaud.Items.Add(baud.Key);
            }
            cmbBaud.SelectedIndex = 0;
        }

        /// <summary>
        /// Handler of FormClosing for safe disconnection from bus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProtocolHandler.Disconnect();
        }

        /// <summary>
        /// Handler of MouseUp event for tracing actual trackbar value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trckAnalog_MouseUp(object sender, MouseEventArgs e)
        {
            if (ProtocolHandler.IsServer)
            {
                ProtocolHandler.ServerUpdateValue('A', trckAnalog.Value);
            }
            return;
        }

        /// <summary>
        /// Handler of Click event for tracing actual button state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttnDigitial_Click(object sender, EventArgs e)
        {
            if (ProtocolHandler.IsServer)
            {
                if (bttnDigitial.BackColor != Color.Green)
                {
                    bttnDigitial.BackColor = Color.Green;
                    ProtocolHandler.ServerUpdateValue('D', Convert.ToInt32(true));
                }
                else
                {
                    bttnDigitial.BackColor = Color.Gray;
                    ProtocolHandler.ServerUpdateValue('D', Convert.ToInt32(false));
                }
            }
        }
    }
}
