using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPS_Modbus
{
    public enum ModbusType
    {
        ASCII,
        RTU
    }

    public enum ModbusErrorCode : byte
    {
        IllegalFunction = 0x1,
        IllegalDataAddress = 0x2,
        IllegalDataValue = 0x3,
        SlaveDeviceFailure = 0x4
    }

    public enum ModbusStatus
    {
        Success,
        InvalidSlaveId,
        IvalidFunction,
        ResponseTimedOut,
        InvalidCrc
    }
}
