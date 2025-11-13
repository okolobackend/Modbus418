namespace Modbus418.Interfaces
{
    public interface IModbusDevice
    {
        // Любой модбас-устройство должно иметь список команд
        // (а это статика, а статика в интерфейсе - противоречит идее полиморфизма), 
        // метод их получения для ПУ 
        // и как минимум его можно вкл\выкл
        abstract IReadOnlyDictionary<ushort, string> AvailableCommands { get; }
        string GetCommandsList();
        byte[] CreateOnOffCommand();

    }
}
