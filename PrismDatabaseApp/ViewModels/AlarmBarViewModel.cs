 using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrismDatabaseApp.ViewModels
{
    public class Alarm
    {
        public string Message { get; set; }
        public string Timestamp { get; set; }
    }

    public class AlarmBarViewModel
    {
        private int slurryTankFlag = 3;

        private readonly ITcpSocketService _tcpSocketService;
        public DelegateCommand<Alarm> DeleteAlarmCommand { get; }
        public ObservableCollection<Alarm> Alarms { get; } = new ObservableCollection<Alarm>();
        public DelegateCommand<Alarm> SendDetailsCommand { get; }

        public AlarmBarViewModel(ITcpSocketService tcpSocketService)
        {
            _tcpSocketService = tcpSocketService;
            _tcpSocketService.DataReceived += OnDataReceived;
            DeleteAlarmCommand = new DelegateCommand<Alarm>(DeleteAlarm);
            SendDetailsCommand = new DelegateCommand<Alarm>(SendDetails);
        }
        private void SendDetails(Alarm alarm)
        {
            if (alarm != null)
            {
                // Message와 Timestamp를 다른 곳으로 전송
                Console.WriteLine($"Sending Details: Message={alarm.Message}, Timestamp={alarm.Timestamp}");

                // 예: TCP 전송 로직 또는 다른 서비스 호출
                // _tcpSocketService.SendData($"Message: {alarm.Message}, Timestamp: {alarm.Timestamp}");
            }
        }
        private void DeleteAlarm(Alarm alarm)
        {
            if (alarm != null)
            {
                Alarms.Remove(alarm);
            }
        }
        private void OnDataReceived(string data)
        {
            try
            {
                var jsonData = JObject.Parse(data);

                // SlurryTank 데이터 처리
                if (jsonData.ContainsKey("SlurryTank"))
                {
                    var slurryData = jsonData["SlurryTank"];
                    var remainingVolume = slurryData["RemainingVolume"]?.Value<double>() ?? 0;
                    var time = slurryData["Timestamp"]?.ToString();

                    if (remainingVolume < 99) // 잔여량이 30L 이하
                    {
                        if (slurryTankFlag > 0) // 플래그가 0 이상일 경우에만 알람 발생
                        {
                            slurryTankFlag--; // 플래그 감소

                            App.Current.Dispatcher.Invoke(() =>
                                Alarms.Add(new Alarm
                                {
                                    Message = $"잔여량이 임계치 미만입니다. 현재 잔여량 - {remainingVolume}L",
                                    Timestamp = time
                                }));
                        }
                    }
                    else // 잔여량이 10L 이상으로 회복
                    {
                        slurryTankFlag = 3; // 플래그 초기화
                    }
                }
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Invoke(() =>
                    Alarms.Add(new Alarm
                    {
                        Message = $"데이터 처리 중 오류 발생: {ex.Message}",
                        Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    }));
            }
        }

    }

}
