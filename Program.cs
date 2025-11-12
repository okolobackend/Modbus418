using Modbus418.MockDevices;

namespace Modbus418
{
    class Program
    {
        static async Task Main()
        {
            using var kettleMock = new KettleMock();
            kettleMock.StartDevice();

            while (true)
            {
                Console.WriteLine("Выберите команду для чайника:");
                Console.WriteLine("0 - Выйти из программы");
                Console.WriteLine("1 - Включить чайник");
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
                        await kettleMock.ProcessModbusCommand();
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }
        }
    }
}
