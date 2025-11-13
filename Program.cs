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
            // Вот тутава больше всего начинается от добавления устройств 
            // до переписывания пульта управления
            using var humidifierMock = new HumidifierMock();
            humidifierMock.StartDevice();

            var kettleModbus = new KettleModbus(usingProtocol);
            var humidifierModbus = new HumidifierModbus(usingProtocol);

            while (true)
            {
                Console.WriteLine("Выберите устройство для управления:");
                Console.WriteLine("0 - Выйти из программы");
                Console.WriteLine("1 - Чайник");
                Console.WriteLine("2 - Увлажнитель");
                Console.Write("Номер устройства: ");
                if (!ushort.TryParse(Console.ReadLine(), out ushort deviceId))
                {
                    Console.WriteLine("Ошибка ввода. Введите число");
                    continue;
                }

                switch(deviceId)
                {
                    case 1:
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
                            case 2:
                                Console.Write("Введите значение температуры нагрева от 20 до 100: ");
                                if (ushort.TryParse(Console.ReadLine(), out ushort maxTemp) && maxTemp >= 20 && maxTemp <= 100)
                                {
                                    var tempData = kettleModbus.SetTemperatureCommand(maxTemp);
                                    await kettleMock.ProcessModbusCommand(tempData);
                                }
                                else
                                {
                                    Console.WriteLine("Ошибка ввода.");
                                }
                                break;
                            default:
                                Console.WriteLine("Неизвестная команда");
                                break;
                        }
                        }
                        
                        break;
                    case 2:
                        {
                            Console.WriteLine("Выберите команду для увлажнителя:");
                        Console.WriteLine("0 - Выйти из программы");
                        Console.WriteLine(humidifierModbus.GetCommandsList());
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
                                var onOffData = humidifierModbus.CreateOnOffCommand();
                                await humidifierMock.ProcessModbusCommand(onOffData);
                                break;
                            case 2:
                                Console.Write("Введите значение для влажности от 30 до 60: ");
                                if (ushort.TryParse(Console.ReadLine(), out ushort targetHumidity) && targetHumidity >= 30 && targetHumidity <= 60)
                                {
                                    var tempData = humidifierModbus.SetHumidityCommand(targetHumidity);
                                    await humidifierMock.ProcessModbusCommand(tempData);
                                }
                                else
                                {
                                    Console.WriteLine("Ошибка ввода.");
                                }
                                break;
                            default:
                                Console.WriteLine("Неизвестная команда");
                                break;
                        }
                        }
                        
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;

                }

 
            }
        }
    }
}
