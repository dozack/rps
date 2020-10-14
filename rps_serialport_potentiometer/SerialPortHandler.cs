using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace rps_serialport_potentiometer
{
    /// <summary>
    /// Physical layer driver
    /// </summary>
    public class SerialPortHandler
    {
        /// <summary>
        /// Serial port instance
        /// </summary>
        private SerialPort _port { get; set; } = new SerialPort();

        private byte[] _buffer { get; set; } = new byte[255];

        private int _index { get; set; } = 0;

        private Thread _thread { get; set; }

        /// <summary>
        /// Serial port status flag, true if port is open
        /// </summary>
        public bool Connected { get { return _port.IsOpen; } set { return; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration structure</param>
        public SerialPortHandler(Configuration config)
        {
            _port = new SerialPort()
            {
                PortName = config.PortName,
                BaudRate = config.BaudRate
            };
            //_port.DataReceived += DataReceived;
            _thread = new Thread(ReceivePolling);
        }

        /// <summary>
        /// Connect to serial port
        /// </summary>
        /// <returns>true if success, false if port is already open, null if error</returns>
        public bool? Connect()
        {
            if (!_port.IsOpen)
            {
                try
                {
                    _port.Open();
                    _port.DiscardInBuffer();
                    _thread.Start();
                    Connected = true;
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
            if (_port.IsOpen)
            {
                try
                {
                    _thread.Abort();
                    _port.Close();
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
            if (_port.IsOpen)
            {
                try
                {
                    byte[] _data = { data };
                    _port.Write(_data, 0, 1);
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
            if (_port.IsOpen)
            {
                try
                {
                    _port.Write(data, 0, data.Length);
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

        void ReceivePolling()
        {
            while (true)
            {
                if(_port.BytesToRead > 0)
                {
                    byte data = Convert.ToByte(_port.ReadByte());
                    if (data == 'a')
                    {
                        Debug.WriteLine("Analog: " + System.Text.Encoding.Default.GetString(_buffer) + "\r\n");
                        OnDataReceivedTrigger('a', _buffer);
                        _buffer.Clear();
                        _index = 0;
                    }
                    else if (data == 'd')
                    {
                        Debug.WriteLine("Digital: " + System.Text.Encoding.Default.GetString(_buffer) + "\r\n");
                        OnDataReceivedTrigger('d', _buffer);
                        _buffer.Clear();
                        _index = 0;
                    }
                    else
                    {
                        _buffer[_index++] = data;
                    }
                }
            }
        }

        /// <summary>
        /// Delegate for sending received data upper layers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void OnDataReceived(SerialPortHandler sender, char type, byte[] data);

        /// <summary>
        /// Event for triggering delegate method
        /// </summary>
        public event OnDataReceived OnDataReceivedEvent;

        /// <summary>
        /// Virtual method for triggering onDataReceived event
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnDataReceivedTrigger(char type, byte[] data) { OnDataReceivedEvent?.Invoke(this, type, data); }
    }
}
