using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace RPS_Modbus
{
    public partial class ModbusWindow : Form
    {
        private Modbus ProtocolHandler;

        private Timer UpdateTimer = new Timer();

        private Dictionary<string, int> baudrates = new Dictionary<string, int>()
        {
            {"9600", 9600 },
            {"19200", 19200},
            {"115200", 115200},
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
                    ClientAddress = 0x1,
                    IsServer = true,
                    ProtocolType = ModbusProtocolType.ASCII,
                };
                // Init new instance of application layer
                ProtocolHandler = new Modbus(config);

                ProtocolHandler.Coils.OnValueUpdated += Coils_OnValueUpdated;
                ProtocolHandler.HoldingRegisters.OnValueUpdated += HoldingRegisters_OnValueUpdated;

                ProtocolHandler.Coils.Write(0x0000, false, true);
                ProtocolHandler.HoldingRegisters.Write(0x0000, 0x0000, true);

                // Try to connect to bus
                if (!ProtocolHandler.Connect())
                {
                    MessageBox.Show("Error during connection, try again.");
                    return;
                }

                if (ProtocolHandler.IsServer)
                {
                    UpdateTimer.Interval = 200;
                    UpdateTimer.Tick += UpdateTimer_Tick;
                    UpdateTimer.Start();
                }

                // Update controls
                bttnConnection.Text = "Disconnect";
                cmbPorts.Enabled = false;
                cmbBaud.Enabled = false;
                grpData.Enabled = true;
            }
            else
            {
                UpdateTimer.Stop();
                // Disconnect from bus
                ProtocolHandler.Disconnect();

                // Update controls
                bttnConnection.Text = "Connect";
                cmbPorts.Enabled = true;
                cmbBaud.Enabled = true;
                grpData.Enabled = false;
            }
        }

        private void HoldingRegisters_OnValueUpdated(ModbusHoldingRegisters sender, ushort address, ushort value)
        {
            if (address == 0x0000)
            {
                Invoke(new Action(() =>
                {
                    trck_Holding.Value = value;
                    lblHoldingVal.Text = value.ToString();
                }));
            }
        }

        private void Coils_OnValueUpdated(ModbusCoils sender, ushort address, bool value)
        {
            if (address == 0x0000)
            {
                Invoke(new Action(() =>
                {
                    bttnCoil.BackColor = value == true ? Color.Green : Color.White;
                }));
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            ProtocolHandler.ReadCoils(0x0000);
            ProtocolHandler.WriteHoldingRegister(0x0000, Convert.ToUInt16(trck_Holding.Value));
        }

        private void trck_Holding_MouseUp(object sender, MouseEventArgs e)
        {
            lblHoldingVal.Text = trck_Holding.Value.ToString();
            if (!ProtocolHandler.IsServer)
            {
                ProtocolHandler.HoldingRegisters.Write(0x0000, Convert.ToUInt16(trck_Holding.Value), false);
            }
        }

        private void bttnCoil_Click(object sender, EventArgs e)
        {
            if (ProtocolHandler.IsServer)
            {
                return;
            }
            bool coilStatus = !Convert.ToBoolean(ProtocolHandler.Coils.Read(0x0000));
            ProtocolHandler.Coils.Write(0x0000, coilStatus, false);
            bttnCoil.BackColor = coilStatus == true ? Color.Green : Color.White;
        }

        private void ModbusWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ProtocolHandler != null && ProtocolHandler.Connected)
            {
                ProtocolHandler.Disconnect();
            }
        }
    }
}
