using Modbus418.Interfaces;

namespace Modbus418.MockDevices
{
    public abstract class BaseMock : IMockDevice
    {
        protected readonly Random _random = new Random();
        protected readonly object _lock = new object();
        protected bool _isOn = false;
        protected bool _isDisposed = false;
        protected CancellationTokenSource? _cts;
        public bool IsOn
        {
            get { lock (_lock) return _isOn; }
        }
        public abstract void StartDevice();
        public void OnOff()
        {
            lock (_lock)
                {
                    if (!_isOn)
                    {
                        _isOn = true;
                        Console.WriteLine("Устройство включено");
                    }
                    else
                    {
                        _isOn = false;
                        Console.WriteLine("Устройство выключено");
                    }
                }
        }
        
        public abstract Task ProcessModbusCommand(byte[] tcpData);
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                Console.WriteLine("Устройство отключено от сети");
                _isDisposed = true;
            }
        }
    }
}