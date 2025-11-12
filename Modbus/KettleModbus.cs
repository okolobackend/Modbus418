using System.Security.Cryptography.X509Certificates;

namespace Modbus418.Modbus
{
    public class KettleModbus
    {
        private bool _isOn = false;
        // Адрес устройства в сети
        public const byte DEVICE_ADDRESS = 0x01;
        // Адрес памяти, катушки, код функции что способны включать устройство
        public const ushort COIL_ON_OFF = 0x0001;
        public const byte FUNCTION_WRITE_SINGLE_COIL = 0x05;
        public static readonly Dictionary<ushort, string> AvailableCommands = new Dictionary<ushort, string>
        {
            {1, "Включить/выключить чайник"},
        };
        public static string GetCommandsList()
        {
            var commands = new List<string>();
            foreach (var cmd in AvailableCommands)
            {
                commands.Add($"{cmd.Key} - {cmd.Value}");
            }
            return string.Join("\n", commands);
        }
        public byte[] CreateOnOffCommand()
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

            return CreateRequestToDevice(pdu);
        }

        public static byte[] CreateRequestToDevice(byte[] pdu)
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
            frame[6] = DEVICE_ADDRESS;

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