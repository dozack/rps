using System;
using System.Diagnostics;
using System.IO.Ports;

namespace RPS_Modbus
{
    /// <summary>
    /// Serial port abstraction layer
    /// </summary>
    public class ModbusSerialStream
    {
        /// <summary>
        /// Physical layer driver instance
        /// </summary>
        private readonly SerialPort Port = new SerialPort();

        /// <summary>
        /// Buffer for storing received bytes
        /// </summary>
        private readonly RingBuffer<byte> Buffer = new RingBuffer<byte>(256);

        /// <summary>
        /// Serial port status flag, true if port is open
        /// </summary>
        public bool Connected { get { return Port.IsOpen; } set { return; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration structure</param>
        public ModbusSerialStream(Configuration config)
        {
            Port = new SerialPort()
            {
                PortName = config.PortName,
                BaudRate = config.BaudRate
            };
            Port.DataReceived += Port_DataReceivedHandler;
        }

        /// <summary>
        /// Connect to serial port
        /// </summary>
        /// <returns>true if success, false if error</returns>
        public bool Connect()
        {
            if (!Connected)
            {
                try
                {
                    Port.Open();
                    Port.DiscardOutBuffer();
                    return true;
                }
                catch
                {
                    return false;
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
        /// <returns>true if success, false if error</returns>
        public bool Disconnect()
        {
            if (Connected)
            {
                try
                {
                    Port.DiscardOutBuffer();
                    Port.Close();
                    return true;
                }
                catch
                {
                    return false;
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
        /// <returns>true if success, false if error</returns>
        public bool Transmit(byte data)
        {
            if (Connected)
            {
                try
                {
                    byte[] _data = { data };
                    Port.Write(_data, 0, 1);
                    return true;
                }
                catch
                {
                    return false;
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
        /// <returns>true if success, false if error</returns>
        public bool Transmit(byte[] data)
        {
            if (Connected)
            {
                try
                {
                    Port.Write(data, 0, data.Length);
                    return true;
                }
                catch
                {
                    return false;
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
                return Buffer.Dequeue();
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
            int n_bytes = Port.BytesToRead;
            for (int i = 0; i < n_bytes; i++)
            {
                int value = Port.ReadByte();
                if (value != -1)
                {
                    Buffer.Enqueue(Convert.ToByte(value));
                }
            }
        }
    }
}
