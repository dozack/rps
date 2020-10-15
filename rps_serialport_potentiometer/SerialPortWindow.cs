using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;

namespace rps_serialport_potentiometer
{
    public partial class SerialPortWindow : Form
    {
        LinkLayer handler;

        private Dictionary<string, int> baudrates = new Dictionary<string, int>()
        {
            {"9600", 9600 },
            {"19200", 57600},
            {"115200", 115200}
        };

        public SerialPortWindow()
        {
            InitializeComponent();
        }

        private void bttnConnection_Click(object sender, EventArgs e)
        {
            if (handler == null || !handler.Connected)
            {
                Configuration config = new Configuration()
                {
                    PortName = cmbPorts.SelectedItem.ToString(),
                    BaudRate = baudrates[cmbBaud.SelectedItem.ToString()],
                    IsMaster = chkMaster.Checked,
                };
                handler = new LinkLayer(config);

                handler.Connect();

                bttnConnection.Text = "Disconnect";
                cmbPorts.Enabled = false;
                cmbBaud.Enabled = false;
                chkMaster.Enabled = false;
            }
            else
            {
                handler.Disconnect();

                bttnConnection.Text = "Connect";
                cmbPorts.Enabled = true;
                cmbBaud.Enabled = true;
                chkMaster.Enabled = true;
            }
        }

        private void SerialPortWindow_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmbPorts.Items.Add(port);
            }
            if(cmbPorts.Items.Count > 0)
            {
                cmbPorts.SelectedIndex = 0;
            }
            foreach( KeyValuePair<string, int> baud in baudrates)
            {
                cmbBaud.Items.Add(baud.Key);
            }
            cmbBaud.SelectedIndex = 0;
        }

        private void SerialPortWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            handler.Disconnect();
        }
    }
}
