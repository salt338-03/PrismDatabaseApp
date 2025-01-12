using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using PrismDatabaseApp.Views;
using Prism.DryIoc;
using Prism.Regions;
using PrismDatabaseApp.ViewModels;

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
            containerRegistry.RegisterForNavigation<DryingProcessView>();
            containerRegistry.RegisterForNavigation<NavigationBarView>();
            containerRegistry.RegisterForNavigation<SlurrySupplyProcessView, SlurrySupplyProcessViewModel>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // 초기 View 설정
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("NavigationRegion", typeof(NavigationBarView));
            regionManager.RequestNavigate("ContentRegion", "SlurrySupplyProcessView");
        }
    }
}
