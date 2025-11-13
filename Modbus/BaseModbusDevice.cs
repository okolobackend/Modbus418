using Modbus418.Interfaces;

namespace Modbus418.Modbus
{
    public abstract class BaseModbusDevice : IModbusDevice
    {
        protected bool _isOn = false;
        // Адрес устройства в сети
        public const byte DEVICE_ADDRESS = 0x01;

        // Для дальнейших разных кодировок нужна зависимость
        protected readonly IModbusProtocol _protocol;
        // Внедрение зависимости через конструктор
        public BaseModbusDevice(IModbusProtocol protocol){
            _protocol = protocol;
        }
        public abstract IReadOnlyDictionary<ushort, string> AvailableCommands { get; }
        public string GetCommandsList()
        {
            var commands = new List<string>();
            foreach (var cmd in AvailableCommands)
            {
                commands.Add($"{cmd.Key} - {cmd.Value}");
            }
            return string.Join("\n", commands);
        }
        public abstract byte[] CreateOnOffCommand();
    }
}