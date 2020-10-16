using System;
using System.IO.Ports;

namespace rps_serialport_potentiometer
{
    /// <summary>
    /// Serial port abstraction layer
    /// </summary>
    public class PhysicalLayer
    {
        /// <summary>
        /// Physical layer driver instance
        /// </summary>
        private readonly SerialPort port = new SerialPort();

        /// <summary>
        /// Buffer for sotring received bytes
        /// </summary>
        private readonly RingBuffer<byte> buffer = new RingBuffer<byte>(256);

        /// <summary>
        /// Thread for polling for received data
        /// </summary>
        //private readonly Thread phy_thread;

        /// <summary>
        /// Serial port status flag, true if port is open
        /// </summary>
        public bool Connected { get { return port.IsOpen; } set { return; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration structure</param>
        public PhysicalLayer(Configuration config)
        {
            port = new SerialPort()
            {
                PortName = config.PortName,
                BaudRate = config.BaudRate
            };
            port.DataReceived += Port_DataReceivedHandler;
        }

        /// <summary>
        /// Connect to serial port
        /// </summary>
        /// <returns>true if success, false if port is already open, null if error</returns>
        public bool? Connect()
        {
            if (!Connected)
            {
                try
                {
                    port.Open();
                    port.DiscardOutBuffer();
                    //phy_thread.Start();
                    return true;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Disconnect from serial port 
        /// </summary>
        /// <returns>true if success, false if port is already closed, null if error</returns>
        public bool? Disconnect()
        {
            if (Connected)
            {
                try
                {
                    port.Close();
                    return true;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Transmit single byte over serial port
        /// </summary>
        /// <param name="data">Byte to send</param>
        /// <returns>true if success, false if port is closed, null if error</returns>
        public bool? Transmit(byte data)
        {
            if (Connected)
            {
                try
                {
                    byte[] _data = { data };
                    port.Write(_data, 0, 1);
                    return true;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Transmit multiple bytes over serial port
        /// </summary>
        /// <param name="data">Data buffer to be transmitted</param>
        /// <returns>true if success, false if port is closed, null if error</returns>
        public bool? Transmit(byte[] data)
        {
            if (Connected)
            {
                try
                {
                    port.Write(data, 0, data.Length);
                    return true;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get single byte cast to in from receive buffer
        /// </summary>
        /// <returns>-1 if buffer empty, else popped value</returns>
        public int Receive()
        {
            try
            {
                return buffer.Dequeue();
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Handler for DataReceived event of serial port, stores received bytes to ring buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Port_DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int n_bytes = port.BytesToRead;
            for (int i = 0; i < n_bytes; i++)
            {
                int value = port.ReadByte();
                if (value != -1)
                {
                    buffer.Enqueue(Convert.ToByte(value));
                }
            }
        }
    }
}
