using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Timers;
using PrismDatabaseApp;

public class SlurrySupplyProcessViewModel : BindableBase
{
    public SeriesCollection SpeedChartSeries { get; set; }
    public Func<double, string> XAxisFormatter { get; set; }

    private double _xAxisMin;
    public double XAxisMin 
    {
        get => _xAxisMin;
        set => SetProperty(ref _xAxisMin, value);
    }

    private double _xAxisMax;
    public double XAxisMax
    {
        get => _xAxisMax;
        set => SetProperty(ref _xAxisMax, value);
    }

    private DateTime _startTime;
    private Timer _dataGenerationTimer;

    public SlurrySupplyProcessViewModel()
    {
        // X축 포맷터 설정 (시간 표시)
        XAxisFormatter = value => _startTime.AddSeconds(value).ToString("hh:mm:ss");

        // 데이터 시리즈 초기화
        SpeedChartSeries = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Speed (mL/min)",
                Values = new ChartValues<ObservablePoint>(),
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 2
            }
        };

        // 초기화
        _startTime = DateTime.Now;
        XAxisMin = 0;
        XAxisMax = 1; // 초기값

        // 랜덤 데이터 생성 타이머 (3초마다 실행)
        _dataGenerationTimer = new Timer(1000); // 3000ms = 3초
        _dataGenerationTimer.Elapsed += GenerateRandomData;
        _dataGenerationTimer.Start();
    }

    /// <summary>
    /// 랜덤 데이터를 생성하고 그래프에 추가
    /// </summary>
    private void GenerateRandomData(object sender, ElapsedEventArgs e)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            Random random = new Random();

            // 랜덤 속도 생성 (0.5 ~ 2.0 mL/min)
            double randomSpeed = Math.Round(random.NextDouble() * 1.5 + 0.5, 2);

            // 현재 시간
            DateTime currentTime = DateTime.Now;

            // 데이터 추가
            AddData(randomSpeed, currentTime);
        });
    }

    /// <summary>
    /// 속도와 시간 데이터를 그래프에 추가
    /// </summary>
    public void AddData(double speed, DateTime timestamp)
    {
        // 시간 경과를 초 단위로 변환
        double elapsedTime = (timestamp - _startTime).TotalSeconds;

        // 그래프에 데이터 추가 (X: 시간 경과, Y: 속도)
        SpeedChartSeries[0].Values.Add(new ObservablePoint(elapsedTime, speed));

        // 오래된 데이터 제거
        if (SpeedChartSeries[0].Values.Count > 6) // 최신 30개 데이터 유지
            SpeedChartSeries[0].Values.RemoveAt(0);

        // X축 범위 업데이트
        XAxisMin = SpeedChartSeries[0].Values.Cast<ObservablePoint>().Min(p => p.X);
        XAxisMax = SpeedChartSeries[0].Values.Cast<ObservablePoint>().Max(p => p.X);

        // UI 업데이트
        RaisePropertyChanged(nameof(SpeedChartSeries));
        RaisePropertyChanged(nameof(XAxisMin));
        RaisePropertyChanged(nameof(XAxisMax));
    }
}
