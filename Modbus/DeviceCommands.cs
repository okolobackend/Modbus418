namespace Modbus418.Modbus
{
    public static class DeviceCommands
    {
        public static class Kettle
        {
            public static readonly IReadOnlyDictionary<ushort, string> Commands =
            new Dictionary<ushort, string>
            {
                [0x0001] = "Включить/выключить чайник",
                [0x0002] = "Установить температуру нагрева"
            }.AsReadOnly();
        }
    }
}