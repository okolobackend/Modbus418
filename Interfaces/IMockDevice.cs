namespace Modbus418.Interfaces
{
    public interface IMockDevice : IDisposable
    {
        // Базово устройства при включении программы "подключаются к сети". 
        // И их можно включить и выключить. И они обрабатывают входящие команды
        void StartDevice();
        void OnOff();
        public Task ProcessModbusCommand(byte[] tcpData);
    }
}