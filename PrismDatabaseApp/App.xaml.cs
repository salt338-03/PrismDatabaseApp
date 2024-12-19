using System.Windows;
using Prism.Ioc;
using PrismDatabaseApp.Data;
using PrismDatabaseApp.Services;
using PrismDatabaseApp.Views;
using Microsoft.EntityFrameworkCore;

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
            return Container.Resolve<MainWindowView>(); // DI를 통해 MainWindow 반환
        }

        /// <summary>
        /// 서비스 및 DbContext를 DI 컨테이너에 등록합니다.
        /// </summary>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            string sql = "Server=SUNJIN-NOTEBOOK\\MSSQLSERVERR;Database=BakingManagementDB;Trusted_Connection=True;TrustServerCertificate=True;";
            // DbContextOptions 설정 후 AppDbContext 등록
            containerRegistry.RegisterSingleton<AppDbContext>(() =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(sql);
                return new AppDbContext(optionsBuilder.Options);
            });

            // 서비스 클래스 등록
            containerRegistry.RegisterSingleton<BakingProductService>();
        }
    }
}

