using LiveCharts;
using LiveCharts.Wpf;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PrismDatabaseApp.ViewModels
{
    public class PLCChartViewModel : BindableBase
    {
        private double _currentValue;
        private double _changeRate;
        private bool _increasing;
        private DispatcherTimer _timer;
        private ChartValues<double> _values;

        public SeriesCollection SeriesCollection { get; }
        public ObservableCollection<string> Labels { get; }

        public double AxisMax { get; set; } = 50;

        public PLCChartViewModel()
        {
            _currentValue = 50.0; // 초기값
            _changeRate = 5;    // 값 변화 속도
            _increasing = true;  // 값 상승 여부

            _values = new ChartValues<double>();
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Real-Time Data",
                    Values = _values
                }
            };

            Labels = new ObservableCollection<string>();

            // Timer to update chart data
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += async (s, e) =>
            {
                if (IsServerAvailable())
                {
                    await UpdateChartAsync();
                }
                else
                {
                    Console.WriteLine("Waiting for server to become available...");
                }
            };
            _timer.Start();
        }

        private async Task UpdateChartAsync()
        {
            try
            {
                double newValue = await Task.Run(() => GenerateRandomValue()); // 랜덤값 생성
                _values.Add(newValue);
                Labels.Add(DateTime.Now.ToString("HH:mm:ss"));

                if (_values.Count > AxisMax) // 데이터 개수가 AxisMax를 초과하면 제거
                {
                    _values.RemoveAt(0);
                    Labels.RemoveAt(0);
                }

                RaisePropertyChanged(nameof(SeriesCollection));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating chart: {ex.Message}");
            }
        }

        private double GenerateRandomValue()
        {
            var random = new Random();

            // 랜덤값 생성 (점진적으로 증가 또는 감소)
            if (_increasing)
            {
                _currentValue += _changeRate * random.NextDouble();
                if (_currentValue >= 100) _increasing = false;
            }
            else
            {
                _currentValue -= _changeRate * random.NextDouble();
                if (_currentValue <= 0) _increasing = true;
            }

            Console.WriteLine($"Current Value: {_currentValue:F2}");
            return _currentValue; // 계산된 값을 반환
        }

        private bool IsServerAvailable()
        {
            try
            {
                using (var client = new TcpClient("127.0.0.1", 502))
                {
                    return true; // 서버에 연결 가능
                }
            }
            catch (Exception)
            {
                return false; // 서버에 연결할 수 없음
            }
        }
    }
}
