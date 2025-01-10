using System.Windows;
using System.Windows.Controls;
using Prism.Regions;

namespace PrismDatabaseApp.Views
{
    public partial class MainWindowView : Window
    {
        private readonly IRegionManager _regionManager;

        public MainWindowView(IRegionManager regionManager)
        {
            InitializeComponent();
        }
    }
}
