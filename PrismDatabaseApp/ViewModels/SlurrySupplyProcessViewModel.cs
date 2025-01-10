using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace PrismDatabaseApp.ViewModels
{
    public class SlurrySupplyProcessViewModel : BindableBase
    {
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
    }
}
