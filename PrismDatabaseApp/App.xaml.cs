using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using PrismDatabaseApp.Views;
using Prism.DryIoc;
using Prism.Regions;
using PrismDatabaseApp.ViewModels;
using System;
using System.Threading.Tasks;

namespace PrismDatabaseApp
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindowView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // View와 ViewModel 등록
            containerRegistry.RegisterForNavigation<CoatingProcessView>();
            containerRegistry.RegisterForNavigation<AlarmBarView>();
            containerRegistry.RegisterForNavigation<DryingProcessView>();
            containerRegistry.RegisterForNavigation<NavigationBarView>();
            containerRegistry.RegisterForNavigation<SlurrySupplyProcessView, SlurrySupplyProcessViewModel>();

            // TcpSocketService 싱글톤 등록
            var tcpSocketService = new TcpSocketService();
            tcpSocketService.Configure("127.0.0.1", 8000); // IP와 포트 설정
            tcpSocketService.StartListening(); // 서비스 시작
            containerRegistry.RegisterInstance<ITcpSocketService>(tcpSocketService);
            
            Console.WriteLine($"TcpSocketService instance: {tcpSocketService.GetHashCode()}");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            // DI 컨테이너에서 모든 뷰모델 생성 및 초기화
            var regionManager = Container.Resolve<IRegionManager>();

            // 모든 뷰를 순차적으로 로드
            regionManager.RequestNavigate("ContentRegion", "SlurrySupplyProcessView");
            regionManager.RequestNavigate("ContentRegion", "CoatingProcessView");
            regionManager.RequestNavigate("ContentRegion", "DryingProcessView");

            // 초기 화면으로 복원
            regionManager.RequestNavigate("ContentRegion", "SlurrySupplyProcessView");
            // 초기 View 설정
            
            regionManager.RegisterViewWithRegion("NavigationRegion", typeof(NavigationBarView));
            regionManager.RequestNavigate("ContentRegion", "SlurrySupplyProcessView");
            regionManager.RequestNavigate("AlarmRegion", "AlarmBarView");
        }
    }
}
