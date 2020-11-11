using System;

namespace RPS_Modbus
{
    public class Modbus
    {
        public ModbusCoils Coils = new ModbusCoils();

        public ModbusHoldingRegisters HoldingRegisters = new ModbusHoldingRegisters();

        public readonly bool IsServer;

        public bool Connected { get { return link != null && link.Connected; } }

        public readonly int ClientAddress;

        private readonly IModbusLink link;

        public Modbus(ModbusConfiguration config)
        {
            switch (config.ProtocolType)
            {
                case ModbusProtocolType.ASCII:
                    link = new ModbusLinkAscii(config);
                    break;
                case ModbusProtocolType.RTU:
                    link = new ModbusLinkRtu(config);
                    break;
                default:
                    throw new ArgumentException("INVALID_CONFIG: ProtocolType");
            }
            IsServer = config.IsServer;
            ClientAddress = config.ClientAddress;
            link.OnMessageReceived += ModbusLink_MessageReceivedHandler;
            link.OnTimeoutOccured += ModbusLink_TimeoutHandler;
        }

        public bool Connect()
        {
            return link.Connect();
        }

        public bool Disconnect()
        {
            return link.Disconnect();
        }

        private bool Send(object command, byte[] data)
        {
            byte[] tmpData = new byte[data.Length + 2];
            tmpData[0] = Convert.ToByte(ClientAddress);
            tmpData[1] = Convert.ToByte(command);
            Array.Copy(data, 0, tmpData, 2, data.Length);
            return link.Send(tmpData);
        }

        private void ModbusLink_TimeoutHandler(object sender, EventArgs e)
        {
            return;
        }

        public bool ReadCoils(ushort address)
        {
            lastCmdAddr = address;
            byte[] cmdData = new byte[4] { Convert.ToByte((address & 0xff00) >> 8), Convert.ToByte(address & 0x00ff), 0x0, 0x1 };
            return Send(ModbusCommand.READ_COILS, cmdData);
        }

        public bool WriteHoldingRegister(ushort address, ushort value)
        {
            byte[] cmdData = new byte[4] { Convert.ToByte((address & 0xff00) >> 8), Convert.ToByte(address & 0x00ff), Convert.ToByte((value & 0xff00) >> 8), Convert.ToByte(value & 0x00ff) };
            return Send(ModbusCommand.WRITE_SINGLE_HOLDING_REGISTER, cmdData);
        }

        private ushort lastCmdAddr = 0x0000;

        private void ProcessMessageServer(byte[] message)
        {
            ModbusCommand command = (ModbusCommand)message[1];
            switch (command)
            {
                case ModbusCommand.READ_COILS:
                    try
                    {
                        // Only single coil read implemented
                        bool regVal = Convert.ToBoolean(message[3]);
                        Coils.Write(lastCmdAddr, regVal, true);
                    }
                    catch
                    {
                        break;
                    }
                    break;
                case ModbusCommand.READ_HOLDING_REGISTERS:
                    try
                    {
                        // Only single holding register read implemented
                        ushort regVal = Convert.ToUInt16((message[3] << 8) | message[4]);
                        HoldingRegisters.Write(lastCmdAddr, regVal, true);
                    }
                    catch
                    {
                        break;
                    }
                    break;
                case ModbusCommand.READ_DISCRETE_INPUTS:
                case ModbusCommand.READ_INPUT_REGISTERS:
                case ModbusCommand.WRITE_SINGLE_COIL:
                case ModbusCommand.WRITE_SINGLE_HOLDING_REGISTER:
                    // Unsupported commands for server side - response not expected
                    break;
                default:
                    break;
            }
        }

        private void ProcessMessageClient(byte[] message)
        {
            ModbusCommand command = (ModbusCommand)message[1];
            switch (command)
            {
                case ModbusCommand.READ_COILS:
                    try
                    {
                        ushort regAddr = Convert.ToUInt16((message[2] << 8) | message[3]);
                        ushort valCount = Convert.ToUInt16((message[4] << 8) | message[5]);
                        if (valCount > 1)
                        {
                            // Reading more than one coil is not supported
                            Send(ModbusErrorResponse.READ_COILS, new byte[1] { Convert.ToByte(ModbusErrorCode.ILLEGAL_DATA_VALUE) });
                            break;
                        }
                        byte regVal = Coils.Read(regAddr);
                        Send(ModbusCommand.READ_COILS, new byte[2] { 0x1, regVal });
                    }
                    catch
                    {
                        // Invalid address
                        Send(ModbusErrorResponse.READ_COILS, new byte[1] { Convert.ToByte(ModbusErrorCode.ILLEGAL_DATA_ADDRESS) });
                    }
                    break;
                case ModbusCommand.READ_DISCRETE_INPUTS:
                    // Command not supported
                    Send(ModbusErrorResponse.READ_DISCRETE_INPUTS, new byte[1] { Convert.ToByte(ModbusErrorCode.ILLEGAL_FUNCTION) });
                    break;
                case ModbusCommand.READ_HOLDING_REGISTERS:
                    try
                    {
                        ushort regAddr = Convert.ToUInt16((message[2] << 8) | message[3]);
                        ushort valCount = Convert.ToUInt16((message[4] << 8) | message[5]);
                        if (valCount > 1)
                        {
                            // Reading more than one register is not supported
                            Send(ModbusErrorResponse.READ_HOLDING_REGISTERS, new byte[1] { Convert.ToByte(ModbusErrorCode.ILLEGAL_DATA_VALUE) });
                            break;
                        }
                        ushort regVal = HoldingRegisters.Read(regAddr);
                        byte[] resData = new byte[3] { 0x1, Convert.ToByte((regVal & 0xff00) >> 8), Convert.ToByte(regVal & 0x00ff) };
                        Send(ModbusCommand.READ_HOLDING_REGISTERS, resData);
                    }
                    catch
                    {
                        // Invalid address
                        Send(ModbusErrorResponse.READ_HOLDING_REGISTERS, new byte[1] { Convert.ToByte(ModbusErrorCode.ILLEGAL_DATA_ADDRESS) });
                    }
                    break;
                case ModbusCommand.READ_INPUT_REGISTERS:
                    // Command not supported
                    Send(ModbusErrorResponse.READ_INPUT_REGISTERS, new byte[1] { Convert.ToByte(ModbusErrorCode.ILLEGAL_FUNCTION) });
                    break;
                case ModbusCommand.WRITE_SINGLE_COIL:
                    // Command not supported
                    Send(ModbusErrorResponse.WRITE_SINGLE_COIL, new byte[1] { Convert.ToByte(ModbusErrorCode.ILLEGAL_FUNCTION) });
                    break;
                case ModbusCommand.WRITE_SINGLE_HOLDING_REGISTER:
                    try
                    {
                        ushort regAddr = Convert.ToUInt16((message[2] << 8) | message[3]);
                        ushort regVal = Convert.ToUInt16((message[4] << 8) | message[5]);
                        // Try to read address first to check if valid
                        HoldingRegisters.Read(regAddr);
                        // Write value if no exception was thrown
                        HoldingRegisters.Write(regAddr, regVal, true);
                        // Send mirror of request as response
                        byte[] resData = new byte[4] { message[2], message[3], message[4], message[5] };
                        Send(ModbusCommand.WRITE_SINGLE_HOLDING_REGISTER, resData);
                    }
                    catch
                    {
                        // Invalid address
                        Send(ModbusErrorResponse.WRITE_SINGLE_HOLDING_REGISTER, new byte[1] { Convert.ToByte(ModbusErrorCode.ILLEGAL_DATA_ADDRESS) });
                    }
                    break;
                default:
                    break;
            }
        }

        private void ModbusLink_MessageReceivedHandler(object sender, MessageReceivedArgs e)
        {
            int addr = Convert.ToInt32(e.Message[0]);
            if (addr != ClientAddress)
            {
                // Invalid address
                return;
            }
            if (IsServer)
            {
                ProcessMessageServer(e.Message);
            }
            else
            {
                ProcessMessageClient(e.Message);
            }
        }
    }
}
