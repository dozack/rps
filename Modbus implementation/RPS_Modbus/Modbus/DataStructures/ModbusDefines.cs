using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPS_Modbus
{
    public enum ModbusProtocolType
    {
        ASCII,
        RTU
    }

    public enum ModbusCommand : byte
    {
        READ_COILS = 0x1,
        READ_DISCRETE_INPUTS = 0x2,
        READ_HOLDING_REGISTERS = 0x3,
        READ_INPUT_REGISTERS = 0x4,
        WRITE_SINGLE_COIL = 0x5,
        WRITE_SINGLE_HOLDING_REGISTER = 0x6,
    }

    public enum ModbusErrorResponse : byte
    {
        READ_COILS = 0x81,
        READ_DISCRETE_INPUTS = 0x82,
        READ_HOLDING_REGISTERS = 0x83,
        READ_INPUT_REGISTERS = 0x84,
        WRITE_SINGLE_COIL = 0x85,
        WRITE_SINGLE_HOLDING_REGISTER = 0x83,
    }

    public enum ModbusErrorCode : byte
    {
        ILLEGAL_FUNCTION = 0x1,
        ILLEGAL_DATA_ADDRESS = 0x2,
        ILLEGAL_DATA_VALUE = 0x3,
        CLIENT_DEVICE_FAILURE = 0x4
    }

    public enum ModbusStatus
    {
        SUCCESS,
        INVALID_CLIENT_ID,
        INVALID_FUNCTION,
        RESPONSE_TIMED_OUT,
        INVALID_CHECKSUM
    }
}
