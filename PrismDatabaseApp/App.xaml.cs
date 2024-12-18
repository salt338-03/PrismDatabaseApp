using System.Windows;
using Prism.Ioc;
using PrismDatabaseApp.Data;
using PrismDatabaseApp.Services;
using PrismDatabaseApp.Views;

namespace PrismDatabaseApp
{
    /// <summary>
    /// Prism을 사용한 WPF 애플리케이션의 진입점입니다.
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// MainWindow를 생성합니다.
        /// </summary>
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>(); // DI를 통해 MainWindow 반환
        }

        /// <summary>
        /// 서비스 및 DbContext를 DI 컨테이너에 등록합니다.
        /// </summary>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Entity Framework DbContext 등록
            containerRegistry.RegisterSingleton<AppDbContext>();

            // 서비스 클래스 등록
            containerRegistry.RegisterSingleton<BakingProductService>();
        }
    }
}
