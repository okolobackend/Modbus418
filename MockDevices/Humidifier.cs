using System;
using System.Threading;
using System.Threading.Tasks;

namespace Modbus418.MockDevices
{
    public class HumidifierMock : BaseMock
    {
        private int _currentHumidity;
        private int _targetHumidity = 50;
        // пусть авторежим выравнивания влажность до целевой будет всегда включен
        private bool _autoMode = true;
        private const int MIN_HUMIDITY = 30;
        private const int MAX_HUMIDITY = 65;
        private const int HUMIDITY_RATE = 1;
        private const int UP_INTERVAL = 100000; // 100 sec
        private const int DOWN_INTERVAL = 300000; // 300 sec
        public int CurrentHumidity
        {
            get { lock (_lock) return _currentHumidity; }
        }
        public int TargetHumidity
        {
            get { lock (_lock) return _targetHumidity; }
        }
        public bool IsAutoMode
        {
            get { lock (_lock) return _autoMode; }
        }

        public override void StartDevice()
        {
            _currentHumidity = _random.Next(20, 60);
            _cts = new CancellationTokenSource();

            Task.Run(async () => await HumidityUpdateLoop(_cts.Token));

            Console.WriteLine("Увлажнитель воздуха подключен к сети");
            Console.WriteLine($"Влажность воздуха: {CurrentHumidity}");
        }

        private async Task HumidityUpdateLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // По умолчанию каждые 10 секунд проверяем влажность в помещении. 
                // Если увлажнитель включен, то корректируем. 
                // Если увлажнитель выключен, но активен авторежим, то включаемся и корректируем
                int updateInterval = 10000;
                lock (_lock)
                {
                    if (IsOn)
                    {
                        if (CurrentHumidity == TargetHumidity)
                        {
                            _isOn = false;
                            Console.WriteLine($"Влажность в помещении установлена {CurrentHumidity}");
                        }
                        else if (CurrentHumidity > TargetHumidity)
                        {
                            updateInterval = DOWN_INTERVAL;
                            _currentHumidity -= HUMIDITY_RATE;
                            Console.WriteLine($"Влажность понижена до: {CurrentHumidity}");
                        }
                        else if (CurrentHumidity < TargetHumidity)
                        {
                            updateInterval = UP_INTERVAL;
                            _currentHumidity += HUMIDITY_RATE;
                            Console.WriteLine($"Влажность повышена до: {CurrentHumidity}");
                        }
                    }
                    else if (!IsOn)
                    {
                        if (IsAutoMode)
                    {
                        // Если включен авторежим и текущая влажность +\- на 5 пунктов разнится с целевой, то включаемся
                        if (CurrentHumidity <= TargetHumidity - 5 || CurrentHumidity >= TargetHumidity + 5)
                        {
                            _isOn = true;
                            Console.WriteLine($"Увлажнитель запущен автоматически");
                        }
                    }
                    }
                    
                }
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
                            int targetHumidity = Math.Clamp((int)value, MIN_HUMIDITY, MAX_HUMIDITY);
                            _targetHumidity = targetHumidity;
                            Console.WriteLine($"Целевая влажность воздуха: {TargetHumidity}");
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