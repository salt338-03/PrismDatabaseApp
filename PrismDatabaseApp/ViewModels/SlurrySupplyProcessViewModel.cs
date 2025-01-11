using LiveCharts;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Timers;
using System;
using System.Linq;

namespace PrismDatabaseApp.ViewModels
{
    public class SlurrySupplyProcessViewModel : BindableBase
    {
        private const int MaxDataPoints = 50; // 표시할 최대 데이터 개수
        public SeriesCollection SpeedChartSeries { get; set; }
        public SeriesCollection TemperatureChartSeries { get; set; }
        public SeriesCollection VolumeChartSeries { get; set; }
        public string[] XAxisLabels { get; set; }
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
                    Values = new ChartValues<double> { 0.75, 0.8, 0.85 },
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 2
                }
            };

            TemperatureChartSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Temperature (°C)",
                    Values = new ChartValues<double> { 22.5, 22.7, 22.6 },
                    PointGeometry = DefaultGeometries.Square,
                    StrokeThickness = 2
                }
            };

            VolumeChartSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Volume (L)",
                    Values = new ChartValues<double> { 3.2, 3.4, 3.5 },
                    PointGeometry = DefaultGeometries.Triangle,
                    StrokeThickness = 2
                }
            };

            XAxisLabels = new[] { "9:00 AM", "9:05 AM", "9:10 AM" };

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
                double newSpeed = random.NextDouble() * 0.5 + 0.75; // 0.75 ~ 1.25
                double newTemp = random.Next(22, 25); // 22°C ~ 25°C
                double newVolume = random.NextDouble() + 3.0; // 3.0L ~ 4.0L

                // 데이터 추가
                SpeedChartSeries[0].Values.Add(newSpeed);
                TemperatureChartSeries[0].Values.Add(newTemp);
                VolumeChartSeries[0].Values.Add(newVolume);

                // 최대 데이터 개수 초과 시 오래된 데이터 제거
                if (SpeedChartSeries[0].Values.Count > MaxDataPoints)
                    SpeedChartSeries[0].Values.RemoveAt(0);
                if (TemperatureChartSeries[0].Values.Count > MaxDataPoints)
                    TemperatureChartSeries[0].Values.RemoveAt(0);
                if (VolumeChartSeries[0].Values.Count > MaxDataPoints)
                    VolumeChartSeries[0].Values.RemoveAt(0);

                // X축 시간 레이블 업데이트
                XAxisLabels = XAxisLabels.Skip(1).Concat(new[] { DateTime.Now.ToString("hh:mm:ss") }).ToArray();
                RaisePropertyChanged(nameof(XAxisLabels));
            });
        }
    }
}
