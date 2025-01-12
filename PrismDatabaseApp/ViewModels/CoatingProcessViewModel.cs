using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Timers;

namespace PrismDatabaseApp.ViewModels
{
    public class WaterSupplyData
    {
        public DateTime Time { get; set; }
        public double SupplyRate { get; set; } // 공급 속도
    }

    public class CoatingProcessViewModel : BindableBase
    {
        private readonly Timer _dataGenerationTimer;
        public PlotModel PlotModel { get; private set; }
        public ObservableCollection<WaterSupplyData> WaterSupplyDataList { get; private set; }
        // Coating Process 관련 로직 추가
        public CoatingProcessViewModel()
        {
            WaterSupplyDataList = new ObservableCollection<WaterSupplyData>();

            // PlotModel 초기화
            PlotModel = new PlotModel { Title = "Water Supply Rate Over Time" };
            // X축 추가
            var dateTimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "HH:mm:ss",
                Title = "Time",
                IntervalType = DateTimeIntervalType.Seconds,
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            PlotModel.Axes.Add(dateTimeAxis);
            // Y축 추가
            var linearAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Supply Rate (L/s)"
            };
            PlotModel.Axes.Add(linearAxis);
            // 데이터 시리즈 추가
            var lineSeries = new LineSeries
            {
                Title = "Water Supply Rate",
                MarkerType = MarkerType.Circle
            };
            PlotModel.Series.Add(lineSeries);

            // 타이머 설정
            _dataGenerationTimer = new Timer(1000);
            _dataGenerationTimer.Elapsed += GenerateRandomData;
            _dataGenerationTimer.Start();

        }
        private void GenerateRandomData(object sender, ElapsedEventArgs e)
        {
            // 랜덤 데이터 생성
            var random = new Random();
            var newData = new WaterSupplyData
            {
                Time = DateTime.Now,
                SupplyRate = random.NextDouble() * 100 // 0~100 사이 랜덤 값
            };

            // UI 스레드에서 데이터 추가
            App.Current.Dispatcher.Invoke(() =>
            {
                WaterSupplyDataList.Add(newData);

                // 오래된 데이터 제거 (100개 제한)
                if (WaterSupplyDataList.Count > 100)
                    WaterSupplyDataList.RemoveAt(0);

                // PlotModel 업데이트
                var series = (LineSeries)PlotModel.Series[0];
                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(newData.Time), newData.SupplyRate));

                // 오래된 점 제거
                if (series.Points.Count > 100)
                    series.Points.RemoveAt(0);

                PlotModel.InvalidatePlot(true); // 그래프 업데이트
            });
        }
    }

}
