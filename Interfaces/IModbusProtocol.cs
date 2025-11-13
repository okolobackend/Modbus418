namespace Modbus418.Interfaces
{
    public interface IModbusProtocol
    {
        // Нужно будет подумать над сигнатурой метода, 
        // так как для рту, вроде, адрес устройства 
        // уже входит в пакет
        byte[] CreateRequestToDevice(byte[] pdu, byte deviceAddress);
    }
}