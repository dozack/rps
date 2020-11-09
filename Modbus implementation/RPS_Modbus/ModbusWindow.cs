using External.MicroTimer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPS_Modbus
{
    public partial class ModbusWindow : Form
    {
        IModbusLink ProtocolHandler;

        private Dictionary<string, int> baudrates = new Dictionary<string, int>()
        {
            {"9600", 9600 },
            {"19200", 57600},
            {"115200", 115200},
            {"921600", 921600 }
        };

        public ModbusWindow()
        {
            InitializeComponent();
        }

        private void ModbusWindow_Load(object sender, EventArgs e)
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

        private void bttnConnection_Click(object sender, EventArgs e)
        {
            if (ProtocolHandler == null || !ProtocolHandler.Connected)
            {
                // ProtocolHandler not initalized or disconnected
                ModbusConfiguration config = new ModbusConfiguration()
                {
                    PortName = cmbPorts.SelectedItem.ToString(),
                    BaudRate = baudrates[cmbBaud.SelectedItem.ToString()],
                };
                // Init new instance of application layer
                ProtocolHandler = new ModbusLinkRtu(config);

                // Try to connect to bus
                if (!ProtocolHandler.Connect())
                {
                    MessageBox.Show("Error during connection, try again.");
                    return;
                }

                //byte[] testData = new byte[] { 0x15, 0x03, 0x00, 0x6B, 0x00, 0x03};
                //ProtocolHandler.Send(testData);

                // Update controls
                bttnConnection.Text = "Disconnect";
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
                cmbPorts.Enabled = true;
                cmbBaud.Enabled = true;
                chkMaster.Enabled = true;
            }
        }
    }
}
