using Prism.Mvvm;
using LiveCharts;
using LiveCharts.Defaults;
using System;
using PrismDatabaseApp.Models;
using PrismDatabaseApp;
using LiveCharts.Wpf;

public class SlurrySupplyProcessViewModel : BindableBase
{
    private readonly ITcpSocketService _tcpSocketService;

    public SeriesCollection SpeedChartSeries { get; set; }
    public SeriesCollection TemperatureChartSeries { get; set; }
    public SeriesCollection VolumeChartSeries { get; set; }

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

    public Func<double, string> XAxisFormatter { get; set; }

    public SlurrySupplyProcessViewModel(ITcpSocketService tcpSocketService)
    {
        _tcpSocketService = tcpSocketService;
        _tcpSocketService.DataReceived += OnDataReceived;
        _tcpSocketService.StartListening(System.Net.IPAddress.Any, 8080);

        _startTime = DateTime.Now;

        // 그래프 데이터 초기화
        SpeedChartSeries = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Speed",
            Values = new ChartValues<ObservablePoint>()
        }
    };

        TemperatureChartSeries = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Temperature",
            Values = new ChartValues<ObservablePoint>()
        }
    };

        VolumeChartSeries = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Volume",
            Values = new ChartValues<ObservablePoint>()
        }
    };

        XAxisMin = 0;
        XAxisMax = 10;

        // X축 포맷터 설정
        XAxisFormatter = value => _startTime.AddSeconds(value).ToString("hh:mm:ss");

    }


    private void OnDataReceived(string data)
    {
        try
        {
            // JSON 데이터를 ProcessData 객체로 변환
            var processData = System.Text.Json.JsonSerializer.Deserialize<ProcessData>(data);

            // SlurryTank의 Timestamp를 DateTime으로 변환
            DateTime timestamp = DateTime.Parse(processData.SlurryTank.Timestamp);

            // X축 시간 계산 (초 단위)
            double elapsedTime = (timestamp - _startTime).TotalSeconds;

            // UI 스레드에서 데이터 추가
            App.Current.Dispatcher.Invoke(() =>
            {
                // Slurry Supply Speed 데이터 추가
                SpeedChartSeries[0].Values.Add(new ObservablePoint(elapsedTime, processData.SlurryTank.SupplySpeed));


                // Slurry Supply Temperature 데이터 추가
                TemperatureChartSeries[0].Values.Add(new ObservablePoint(elapsedTime, processData.SlurryTank.Temperature));

                // Slurry Supply Volume 데이터 추가
                VolumeChartSeries[0].Values.Add(new ObservablePoint(elapsedTime, processData.SlurryTank.RemainingVolume));

                // 오래된 데이터 제거 (최신 30개 데이터 유지)
                if (SpeedChartSeries[0].Values.Count > 30)
                    SpeedChartSeries[0].Values.RemoveAt(0);

                if (TemperatureChartSeries[0].Values.Count > 30)
                    TemperatureChartSeries[0].Values.RemoveAt(0);

                if (VolumeChartSeries[0].Values.Count > 30)
                    VolumeChartSeries[0].Values.RemoveAt(0);

                // X축 범위 업데이트
                XAxisMin = Math.Max(0, elapsedTime - 30); // 최신 30초 범위 유지
                XAxisMax = elapsedTime;
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing data: {ex.Message}");
        }
    }

}
