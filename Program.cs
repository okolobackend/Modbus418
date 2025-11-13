using Modbus418.MockDevices;
using Modbus418.Modbus;

namespace Modbus418
{
    class Program
    {
        static async Task Main()
        {

            var usingProtocol = new ModbusTCP();
            using var kettleMock = new KettleMock();
            kettleMock.StartDevice();
            var kettleModbus = new KettleModbus(usingProtocol);

            while (true)
            {
                Console.WriteLine("Выберите команду для чайника:");
                Console.WriteLine("0 - Выйти из программы");
                Console.WriteLine(kettleModbus.GetCommandsList());
                Console.Write("Номер команды: ");

                if (!ushort.TryParse(Console.ReadLine(), out ushort commandId))
                {
                    Console.WriteLine("Ошибка ввода. Введите число");
                    continue;
                }

                switch (commandId)
                {
                    case 0:
                        Console.WriteLine("Завершение работы");
                        return;

                    case 1:
                        var onOffData = kettleModbus.CreateOnOffCommand();
                        await kettleMock.ProcessModbusCommand(onOffData);
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }
        }
    }
}
