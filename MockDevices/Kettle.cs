using System;
using System.Threading;
using System.Threading.Tasks;

namespace Modbus418.MockDevices
{
    public class KettleMock : BaseDevice
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
        public bool IsOn
        {
            get { lock (_lock) return _isOn; }
        }

        public override void StartDevice()
        {
            _currentTemperature = _random.Next(20, 90);
            _cts = new CancellationTokenSource();

            Task.Run(async () => await TemperatureUpdateLoop(_cts.Token));

            Console.WriteLine("Чайник подключен к сети");
            Console.WriteLine($"Температура воды в чайнике: {_currentTemperature}");
        }

        private async Task TemperatureUpdateLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {

                lock (_lock)
                {
                    // Нагрев чайника и автоматическое отключение по достижению максимальной температуры
                    if (_isOn && _currentTemperature < _maxHeatingTemperature)
                    {
                        _currentTemperature = Math.Min(_currentTemperature + HEATING_RATE, _maxHeatingTemperature);
                        if (_currentTemperature == _maxHeatingTemperature)
                        {
                            _isOn = false;
                            Console.WriteLine("Чайник вскипел");
                        }
                        else if (_currentTemperature > _maxHeatingTemperature)
                        {
                            _isOn = false;
                            Console.WriteLine($"Температура воды {_currentTemperature} превысила максимальное значение {_maxHeatingTemperature}");
                        }
                    }
                    // Остывание до комнатной температуры
                    else if (!_isOn && _currentTemperature > MIN_TEMPERATURE)
                    {
                        _currentTemperature = Math.Max(_currentTemperature - COOLING_RATE, MIN_TEMPERATURE);
                        if (_currentTemperature % 5 == 0)
                        {
                            Console.WriteLine($"Вода остыла до {_currentTemperature}");
                        }
                    }
                }
                // Увеличиваем температуру раз в секунду, охлаждаемся раз в три
                int updateInterval = _isOn ? HEATING_INTERVAL : COOLING_INTERVAL;
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
        // Пока что только вкл/выкл, но название(!) само за себя говорит!
        public async Task ProcessModbusCommand()
        {
            await Task.Run(() =>
            {
                OnOff();
            });
            
        }
    }
}