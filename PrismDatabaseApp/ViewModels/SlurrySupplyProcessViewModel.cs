using LiveCharts;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Timers;
using System;
using System.Linq;
using LiveCharts.Defaults;

namespace PrismDatabaseApp.ViewModels
{

  
    public class SlurrySupplyProcessViewModel : BindableBase
    {
        private DateTime _startTime = DateTime.Now; // 시작 시간 저장
        private const int MaxDataPoints = 50; // 표시할 최대 데이터 개수
        public SeriesCollection SpeedChartSeries { get; set; }
        public SeriesCollection TemperatureChartSeries { get; set; }
        public SeriesCollection VolumeChartSeries { get; set; }
        
        public string[] XAxisLabelsSpeed { get; set; } = Array.Empty<string>(); // 빈 배열로 초기화
        public string[] XAxisLabelsTemperature { get; set; } = Array.Empty<string>(); // 빈 배열로 초기화
        public string[] XAxisLabelsVolume { get; set; } = Array.Empty<string>(); // 빈 배열로 초기화

        private Timer _updateTimer;

        private double _progress;
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private string _estimatedTime;
        public string EstimatedTime
        {
            get => _estimatedTime;
            set => SetProperty(ref _estimatedTime, value);
        }

        private string _slurrySupplySpeed;
        public string SlurrySupplySpeed
        {
            get => _slurrySupplySpeed;
            set => SetProperty(ref _slurrySupplySpeed, value);
        }

        private string _slurrySupplyVolume;
        public string SlurrySupplyVolume
        {
            get => _slurrySupplyVolume;
            set => SetProperty(ref _slurrySupplyVolume, value);
        }

        private string _slurrySupplyPressure;
        public string SlurrySupplyPressure
        {
            get => _slurrySupplyPressure;
            set => SetProperty(ref _slurrySupplyPressure, value);
        }

        private string _slurrySupplyTemperature;
        public string SlurrySupplyTemperature
        {
            get => _slurrySupplyTemperature;
            set => SetProperty(ref _slurrySupplyTemperature, value);
        }

        public ObservableCollection<string> Notifications { get; }

        public SlurrySupplyProcessViewModel()
        {
            SpeedChartSeries = new SeriesCollection
    {
       new LineSeries
    {
        Title = "Speed (mL/min)",
        Values = new ChartValues<ObservablePoint> { new ObservablePoint(0, 0.75) },
        PointGeometry = DefaultGeometries.Circle,
        StrokeThickness = 2
    }
    };

            TemperatureChartSeries = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Temperature (°C)",
            Values = new ChartValues<ObservablePoint>(),
            PointGeometry = DefaultGeometries.Square,
            StrokeThickness = 2
        }
    };

            VolumeChartSeries = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Volume (L)",
            Values = new ChartValues<ObservablePoint>(),
            PointGeometry = DefaultGeometries.Triangle,
            StrokeThickness = 2
        }
            };
            


            // 타이머로 실시간 데이터 업데이트
            _updateTimer = new Timer(1000); // 1초마다 실행
            _updateTimer.Elapsed += UpdateChartData;
            _updateTimer.Start();
            // 초기 데이터 설정
            Progress = 65;
            EstimatedTime = "Estimated time remaining: 3 hours 30 minutes";
            SlurrySupplySpeed = "0.75 mL/min";
            SlurrySupplyVolume = "3.2 L";
            SlurrySupplyPressure = "1.8 bar";
            SlurrySupplyTemperature = "22.5°C";
            // 초기 데이터 추가
            var initialTime = (DateTime.Now - _startTime).TotalSeconds;
            XAxisLabelsSpeed = new[] { $"{_startTime:hh:mm:ss}" };
            SpeedChartSeries[0].Values.Add(new ObservablePoint(initialTime, 0.75)); // 초기값 추가
            XAxisLabelsSpeed = new[] { $"{_startTime.AddSeconds(initialTime):hh:mm:ss}" };

            TemperatureChartSeries[0].Values.Add(new ObservablePoint(initialTime, 22.5));
            XAxisLabelsTemperature = new[] { $"{_startTime.AddSeconds(initialTime):hh:mm:ss}" };

            VolumeChartSeries[0].Values.Add(new ObservablePoint(initialTime, 3.2));
            XAxisLabelsVolume = new[] { $"{_startTime.AddSeconds(initialTime):hh:mm:ss}" };

            RaisePropertyChanged(nameof(XAxisLabelsSpeed));
            RaisePropertyChanged(nameof(XAxisLabelsTemperature));
            RaisePropertyChanged(nameof(XAxisLabelsVolume));

            // 알림 데이터
            Notifications = new ObservableCollection<string>
            {
                "Exceeded threshold: Slurry supply speed exceeded 0.7 mL/min (8:55 AM)",
                "Exceeded threshold: Slurry supply pressure dropped below 2.0 bar (8:45 AM)",
                "Exceeded threshold: Slurry supply temperature exceeded 25°C (8:30 AM)"
            };
        }
        private void UpdateChartData(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Random random = new Random();

                // 새로운 데이터 생성
                double newSpeed = Math.Round(random.NextDouble() * 0.5 + 0.75, 5); // 0.75000 ~ 1.25000
                var elapsedTime = (DateTime.Now - _startTime).TotalSeconds; // 경과 시간(초)
                double newTemp = Math.Round(random.Next(22, 25) + random.NextDouble(), 5); // 22.00000 ~ 25.00000
                double newVolume = Math.Round(random.NextDouble() + 3.0, 5); // 3.00000 ~ 4.00000

                // 데이터 추가
                SpeedChartSeries[0].Values.Add(new ObservablePoint(elapsedTime, newSpeed));
                TemperatureChartSeries[0].Values.Add(new ObservablePoint(elapsedTime, newTemp));
                VolumeChartSeries[0].Values.Add(new ObservablePoint(elapsedTime, newVolume));

                // 오래된 데이터 제거 (최대 데이터 개수 초과 시)
                if (SpeedChartSeries[0].Values.Count > MaxDataPoints)
                {
                    SpeedChartSeries[0].Values.RemoveAt(0);
                    XAxisLabelsSpeed = XAxisLabelsSpeed.Skip(1).ToArray(); // 레이블 제거
                }

                if (TemperatureChartSeries[0].Values.Count > MaxDataPoints)
                {
                    TemperatureChartSeries[0].Values.RemoveAt(0);
                    XAxisLabelsTemperature = XAxisLabelsTemperature.Skip(1).ToArray();
                }

                if (VolumeChartSeries[0].Values.Count > MaxDataPoints)
                {
                    VolumeChartSeries[0].Values.RemoveAt(0);
                    XAxisLabelsVolume = XAxisLabelsVolume.Skip(1).ToArray();
                }

                // X축 레이블 업데이트
                XAxisLabelsSpeed = SpeedChartSeries[0].Values
                    .Cast<ObservablePoint>()
                    .Select(p => $"{_startTime.AddSeconds(p.X):hh:mm:ss}")
                    .ToArray();

                XAxisLabelsTemperature = TemperatureChartSeries[0].Values
                    .Cast<ObservablePoint>()
                    .Select(p => $"{_startTime.AddSeconds(p.X):hh:mm:ss}")
                    .ToArray();

                XAxisLabelsVolume = VolumeChartSeries[0].Values
                    .Cast<ObservablePoint>()
                    .Select(p => $"{_startTime.AddSeconds(p.X):hh:mm:ss}")
                    .ToArray();

                // RaisePropertyChanged 호출
                RaisePropertyChanged(nameof(XAxisLabelsSpeed));
                RaisePropertyChanged(nameof(XAxisLabelsTemperature));
                RaisePropertyChanged(nameof(XAxisLabelsVolume));
            });
        }




    }
}
