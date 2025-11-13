using Modbus418.Interfaces;

namespace Modbus418.Modbus
{
    public class KettleModbus : BaseModbusDevice
    {
        // Адрес памяти, катушки, код функции что способны включать устройство и изменять температуру
        public const ushort COIL_ON_OFF = 0x0001;
        public const ushort REGISTER_TEMPERATURE = 0x0000;
        public const byte FUNCTION_WRITE_SINGLE_COIL = 0x05;
        public const byte FUNCTION_WRITE_SINGLE_REGISTER = 0x06;

        public KettleModbus(IModbusProtocol protocol) : base(protocol)
        {
        }

        public override IReadOnlyDictionary<ushort, string> AvailableCommands => DeviceCommands.Kettle.Commands;
        public override byte[] CreateOnOffCommand()
        {
            // Чтобы в центре управления не уточнять включить или выключить чайник, 
            // нам придется запоминать состояние устройства, 
            // что конечно же печалит. 
            // В общем, здесь нужно поправить
            ushort value = (!_isOn) ? (ushort)0xFF00 : (ushort)0x0000;
            if (value == (ushort)0xFF00)
            {
                _isOn = true;
            }

            var pdu = new byte[]
            {
                FUNCTION_WRITE_SINGLE_COIL,
                // Coil address Hi
                (byte)(COIL_ON_OFF >> 8),
                // Coil address Lo
                (byte)(COIL_ON_OFF & 0xFF),
                // Force data Hi
                (byte)(value >> 8),
                // Force data Lo
                (byte)(value & 0xFF)
            };

            return _protocol.CreateRequestToDevice(pdu, DEVICE_ADDRESS);
        }
        
        public byte[] SetTemperatureCommand(ushort maxTemp)
        {
            var pdu = new byte[]
            {
                FUNCTION_WRITE_SINGLE_REGISTER,
                (byte)(REGISTER_TEMPERATURE >> 8),
                (byte)(REGISTER_TEMPERATURE & 0xFF),
                (byte)(maxTemp >> 8),
                (byte)(maxTemp & 0xFF)
            };
            return _protocol.CreateRequestToDevice(pdu, DEVICE_ADDRESS);
        } 
}
}