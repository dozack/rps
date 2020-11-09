using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPS_Modbus
{
    public class ModbusMaster
    {
        private readonly IModbusLink link;

        public bool Connected { get { return link != null && link.Connected; } }

        public ModbusMaster(ModbusConfiguration config)
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
                    throw new InvalidOperationException("Invalid protocol type in configuration.");
            }
            link.OnMessageReceived += ModbusLink_MessageReceived;
            link.OnTimeoutOccured += Link_TimeoutOccured;
        }

        public bool Start()
        {
            return link.Connect();
        }

        public bool Stop()
        {
            return link.Disconnect();
        }

        private void Link_TimeoutOccured(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ModbusLink_MessageReceived(object sender, MessageReceivedArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
