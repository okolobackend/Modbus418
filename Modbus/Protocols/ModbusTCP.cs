using Modbus418.Interfaces;

namespace Modbus418.Modbus
{
    class ModbusTCP : IModbusProtocol
    {
        public byte[] CreateRequestToDevice(byte[] pdu, byte deviceAddress)
        {
            var frame = new byte[7 + pdu.Length];

            // Байты 0-1: Transaction Identifier
            // Используется для сопоставления запросов и ответов
            // В симуляции можно использовать любые значения
            frame[0] = 0x00;
            frame[1] = 0x01;
            // Байты 2-3: Protocol Identifier
            // Всегда 0x0000 для Modbus TCP
            frame[2] = 0x00;
            frame[3] = 0x00;
            // Байты 4-5: Length
            // Количество байтов после этого поля (Unit ID + PDU)
            ushort length = (ushort)(pdu.Length);
            frame[4] = (byte)(length >> 8);
            frame[5] = (byte)(length & 0xFF);
            // Байт 6: Unit Identifier
            // Адрес Modbus устройства
            frame[6] = deviceAddress;

            // Состав pdu
            // Байт 7: Function Code
            // Байты 8-9: Register Address
            // Big-endian порядок: старший байт первый
            // Байты 10-11: Value
            // Для coil: 0xFF00 = ON, 0x0000 = OFF
            // Big-endian порядок
            Array.Copy(pdu, 0, frame, 7, pdu.Length);

            return frame;
        }
    }
}