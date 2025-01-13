using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PrismDatabaseApp.ViewModels;


namespace PrismDatabaseApp.Views
{
    /// <summary>
    /// DryingProcessView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DryingProcessView : UserControl
    {
        public DryingProcessView()
        {
            InitializeComponent();
            DataContextChanged += (s, e) =>
            {
                if (DataContext is DryingProcessViewModel viewModel)
                {
                    // 초기 URL 설정
                    Browser.Navigate(viewModel.Url);

                    // URL 변경 시 Navigate 호출
                    viewModel.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == nameof(DryingProcessViewModel.Url))
                        {
                            Browser.Navigate(viewModel.Url);
                        }
                    };
                }
            };
        }
    }
}
