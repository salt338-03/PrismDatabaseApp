// ViewModels/AlarmBarViewModel.cs
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace PrismDatabaseApp.ViewModels
{
    public class Alarm
    {
        public string Message { get; set; }
        public string Timestamp { get; set; }
    }

    public class AlarmBarViewModel : BindableBase
    {
        private int slurryTankFlag = 3;

        private readonly ITcpSocketService _tcpSocketService;
        private readonly AlarmService _alarmService;  // 인터페이스 없이 직접

        public DelegateCommand<Alarm> DeleteAlarmCommand { get; }
        public DelegateCommand<Alarm> SendDetailsCommand { get; }

        public ObservableCollection<Alarm> Alarms { get; } = new ObservableCollection<Alarm>();

        public AlarmBarViewModel(ITcpSocketService tcpSocketService, AlarmService alarmService)
        {
            _tcpSocketService = tcpSocketService;
            _alarmService = alarmService;  // 주입

            _tcpSocketService.DataReceived += OnDataReceived;

            DeleteAlarmCommand = new DelegateCommand<Alarm>(DeleteAlarm);
            SendDetailsCommand = new DelegateCommand<Alarm>(SendDetails);
        }

        private void SendDetails(Alarm alarm)
        {
            if (alarm != null)
            {
                // 전송 로직
                Console.WriteLine($"Sending Details: {alarm.Message}, {alarm.Timestamp}");
            }
        }

        private void DeleteAlarm(Alarm alarm)
        {
            if (alarm != null)
            {
                Alarms.Remove(alarm);
                // DB 삭제 필요 시 _alarmService.DeleteAlarmAsync(...) 만들 수 있음
            }
        }

        private async void OnDataReceived(string data)
        {
            try
            {
                var jsonData = JObject.Parse(data);

                if (jsonData.ContainsKey("SlurryTank"))
                {
                    var slurryData = jsonData["SlurryTank"];
                    var remainingVolume = slurryData["RemainingVolume"]?.Value<double>() ?? 0;
                    var timeString = slurryData["Timestamp"]?.ToString();

                    DateTime.TryParse(timeString, out DateTime dt);

                    if (remainingVolume < 99)  // 예시 임계치
                    {
                        if (slurryTankFlag > 0)
                        {
                            slurryTankFlag--;

                            // UI에 표시할 Alarm
                            var newAlarm = new Alarm
                            {
                                Message = $"잔여량이 임계치 미만: {remainingVolume}L",
                                Timestamp = timeString
                            };

                            App.Current.Dispatcher.Invoke(() =>
                                Alarms.Add(newAlarm)
                            );

                            // DB 저장 (AlarmService 직접 호출)
                            await _alarmService.SaveAlarmSequentialAsync(
                                newAlarm.Message,
                                dt == DateTime.MinValue ? DateTime.Now : dt
                            );
                        }
                    }
                    else
                    {
                        slurryTankFlag = 3;
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new Alarm
                {
                    Message = $"데이터 처리 중 오류: {ex.Message}",
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                App.Current.Dispatcher.Invoke(() => Alarms.Add(err));

                await _alarmService.SaveAlarmSequentialAsync(
                    err.Message,
                    DateTime.Now
                );
            }
        }
    }
}
