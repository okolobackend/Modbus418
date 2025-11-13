using System;
using System.Threading;
using System.Threading.Tasks;

namespace Modbus418.MockDevices
{
    public class KettleMock : BaseMock
    {
        private int _currentTemperature;
        private int _maxHeatingTemperature = 100;
        private const int MIN_TEMPERATURE = 20;
        private const int MAX_TEMPERATURE = 100;
        private const int HEATING_RATE = 1;
        private const int HEATING_INTERVAL = 1000;
        private const int COOLING_RATE = 1;
        private const int COOLING_INTERVAL = 3000;

        public int CurrentTemperature
        {
            get { lock (_lock) return _currentTemperature; }
        }
        public int MaxHeatingTemperature
        {
            get { lock (_lock) return _maxHeatingTemperature; }
        }

        public override void StartDevice()
        {
            _currentTemperature = _random.Next(20, 90);
            _cts = new CancellationTokenSource();

            Task.Run(async () => await TemperatureUpdateLoop(_cts.Token));

            Console.WriteLine("Чайник подключен к сети");
            Console.WriteLine($"Температура воды в чайнике: {CurrentTemperature}");
        }

        private async Task TemperatureUpdateLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {

                lock (_lock)
                {
                    // Нагрев чайника и автоматическое отключение по достижению максимальной температуры
                    if (IsOn && CurrentTemperature < MaxHeatingTemperature)
                    {
                        _currentTemperature = Math.Min(CurrentTemperature + HEATING_RATE, MaxHeatingTemperature);
                        if (CurrentTemperature == MaxHeatingTemperature)
                        {
                            _isOn = false;
                            Console.WriteLine("Чайник вскипел");
                        }
                        else if (CurrentTemperature > MaxHeatingTemperature)
                        {
                            _isOn = false;
                            Console.WriteLine($"Температура воды {CurrentTemperature} превысила максимальное значение {MaxHeatingTemperature}");
                        }
                    }
                    // Остывание до комнатной температуры
                    else if (!IsOn && CurrentTemperature > MIN_TEMPERATURE)
                    {
                        _currentTemperature = Math.Max(CurrentTemperature - COOLING_RATE, MIN_TEMPERATURE);
                        if (CurrentTemperature % 5 == 0)
                        {
                            Console.WriteLine($"Вода остыла до {CurrentTemperature}");
                        }
                    }
                }
                // Увеличиваем температуру раз в секунду, охлаждаемся раз в три
                int updateInterval = IsOn ? HEATING_INTERVAL : COOLING_INTERVAL;
                try
                {
                    await Task.Delay(updateInterval, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
        public override async Task ProcessModbusCommand(byte[] tcpData)
        {
            await Task.Run(() =>
            {
                if (tcpData.Length < 12)
                {
                    Console.WriteLine("");
                    return;
                }

                byte unitId = tcpData[6];
                byte functionCode = tcpData[7];
                ushort registerAddress = (ushort)((tcpData[8] << 8) | tcpData[9]);
                ushort value = (ushort)((tcpData[10] << 8) | tcpData[11]);

                Console.WriteLine($"Получена команда: [Unit ID: {unitId}, Код функции: {functionCode:X2}, Регистр: {registerAddress:X4}, Значение: {value:X4}]");

                switch (functionCode)
                {
                    case 0x05:
                        OnOff();
                        break;
                    case 0x06:
                        lock (_lock)
                        {
                            int maxTemp = Math.Clamp((int)value, MIN_TEMPERATURE, MAX_TEMPERATURE);
                            _maxHeatingTemperature = maxTemp;
                            Console.WriteLine($"Установлена температура нагрева: {MaxHeatingTemperature}");
                        }
                        break;
                    default:
                        Console.WriteLine($"Код функции: {functionCode} не поддерживается");
                        break;
                }
                
            });
            
        }
    }
}