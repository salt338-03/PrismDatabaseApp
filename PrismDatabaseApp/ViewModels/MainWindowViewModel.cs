using Prism.Commands;
using Prism.Mvvm;
using PrismDatabaseApp.Models;
using PrismDatabaseApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace PrismDatabaseApp.ViewModels
{
    /// <summary>
    /// MainWindow의 ViewModel - 사용자 입력 및 데이터 조회 처리
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private readonly BakingProductService _bakingProductService;

        /// <summary>
        /// UI에 바인딩할 데이터 리스트
        /// </summary>
        public ObservableCollection<BakingProductModel> BakingProducts { get; set; } = new ObservableCollection<BakingProductModel>();

        // 2024년 1월 1일로 초기화
        private DateTime _startDate = new DateTime(2024, 1, 1);
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate = DateTime.Now;
        /// <summary>
        /// 검색 끝 날짜
        /// </summary>
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private string _batchNumber;
        /// <summary>
        /// 검색 배치번호 조건
        /// </summary>
        public string BatchNumber
        {
            get => _batchNumber;
            set => SetProperty(ref _batchNumber, value);
        }

        /// <summary>
        /// 검색 버튼과 연결된 DelegateCommand
        /// </summary>
        public DelegateCommand SearchCommand { get; }

        /// <summary>
        /// ViewModel 생성자 - BakingProductService 의존성 주입
        /// </summary>
        public MainWindowViewModel(BakingProductService bakingProductService)
        {
            _bakingProductService = bakingProductService ?? throw new ArgumentNullException(nameof(bakingProductService), "BakingProductService cannot be null.");

            // Search 메서드와 검색 버튼을 연결
            SearchCommand = new DelegateCommand(async () => await SearchAsync());
        }

        /// <summary>
        /// 비동기 방식으로 데이터를 검색합니다.
        /// </summary>
        private async Task SearchAsync()
        {
            try
            {
                // UI 갱신 전에 기존 데이터를 초기화
                BakingProducts.Clear();

                // 데이터베이스에서 조건에 맞는 데이터를 가져옴
                var results = await Task.Run(() =>
                    _bakingProductService.GetProductsByDateRangeAndBatch(StartDate, EndDate, BatchNumber));

                // 조회된 데이터를 UI에 반영
                foreach (var product in results)
                {
                    BakingProducts.Add(product);
                }

                // 사용자에게 결과 알림
                MessageBox.Show($"{results.Count}개의 결과를 가져왔습니다.", "검색 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // 예외 발생 시 사용자에게 알림
                MessageBox.Show($"데이터를 검색하는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
