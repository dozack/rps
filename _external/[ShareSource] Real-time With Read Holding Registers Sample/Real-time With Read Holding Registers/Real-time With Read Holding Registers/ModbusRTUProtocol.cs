using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Real_time_With_Read_Holding_Registers
{
    public class ModbusRTUProtocol
    {
        private byte slaveAddress = 1;
        private byte function = 3;
        private ushort startAddress = 40001;
        private uint _NumberOfPoints = 3;

        private SerialPort serialPort1 = null;
        private List<Register> _Registers = new List<Register>();

        public ModbusRTUProtocol()
        {
            for (ushort i = 0; i < NumberOfPoints; i++)
            {
                Registers.Add(new Register() { Address = (ushort)(startAddress + i) });
            }


        }

        public void Start()
        {
            try
            {
                serialPort1 = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
                serialPort1.Open();
                ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                {
                    while (true)
                    {
                        if (serialPort1.IsOpen)
                        {
                            byte[] frame = ReadHoldingRegistersMsg(slaveAddress, startAddress, function, NumberOfPoints);
                            serialPort1.Write(frame, 0, frame.Length);
                            Thread.Sleep(100); // Delay 100ms
                            if (serialPort1.BytesToRead >= 5)
                            {
                                byte[] bufferReceiver = new byte[this.serialPort1.BytesToRead];
                                serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                                serialPort1.DiscardInBuffer();

                                // Process data.
                                byte[] data = new byte[bufferReceiver.Length - 5];
                                Array.Copy(bufferReceiver, 3, data, 0, data.Length);
                                UInt16[] result = Word.ByteToUInt16(data);
                                for (int i = 0; i < result.Length; i++)
                                {                                    
                                    Registers[i].Value = result[i];
                                }
                            }
                        }
                        Thread.Sleep(100); // Delay 100ms
                    }
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Function 03 (03hex) Read Holding Registers
        /// Read the binary contents of holding registers in the slave.
        /// </summary>
        /// <param name="slaveAddress">Slave Address</param>
        /// <param name="startAddress">Starting Address</param>
        /// <param name="function">Function</param>
        /// <param name="numberOfPoints">Quantity of inputs</param>
        /// <returns>Byte Array</returns>
        private byte[] ReadHoldingRegistersMsg(byte slaveAddress, ushort startAddress, byte function, uint numberOfPoints)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveAddress;			    // Slave Address
            frame[1] = function;				    // Function             
            frame[2] = (byte)(startAddress >> 8);	// Starting Address High
            frame[3] = (byte)startAddress;		    // Starting Address Low            
            frame[4] = (byte)(numberOfPoints >> 8);	// Quantity of Registers High
            frame[5] = (byte)numberOfPoints;		// Quantity of Registers Low
            byte[] crc = this.CalculateCRC(frame);  // Calculate CRC.
            frame[frame.Length - 2] = crc[0];       // Error Check Low
            frame[frame.Length - 1] = crc[1];       // Error Check High
            return frame;
        }


        /// <summary>
        /// CRC Calculation 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] CalculateCRC(byte[] data)
        {
            ushort CRCFull = 0xFFFF; // Set the 16-bit register (CRC register) = FFFFH.
            char CRCLSB;
            byte[] CRC = new byte[2];
            for (int i = 0; i < (data.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ data[i]); // 

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = (byte)(CRCFull & 0xFF);
            return CRC;
        }

        public List<Register> Registers
        {
            get
            {
                return _Registers;
            }

            set
            {
                _Registers = value;
            }
        }

        public uint NumberOfPoints
        {
            get
            {
                return _NumberOfPoints;
            }

            set
            {
                _NumberOfPoints = value;
            }
        }
    }
}
