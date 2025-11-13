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
        // Первый файл, который нужно править, а не создавать. 
        // Справедливости ради расширение, а не изменение
        public static class Humidifier
        {
            public static readonly IReadOnlyDictionary<ushort, string> Commands =
            new Dictionary<ushort, string>
            {
                [0x0001] = "Включить/выключить увлажнитель воздуха",
                [0x0002] = "Установить целевую влажность воздуха"
            }.AsReadOnly();
        }
    }
}