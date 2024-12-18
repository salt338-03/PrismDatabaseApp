using Prism.Commands;
using Prism.Mvvm;
using PrismDatabaseApp.Models;
using PrismDatabaseApp.Services;
using System;
using System.Collections.ObjectModel;

namespace PrismDatabaseApp.ViewModels
{
    /// <summary>
    /// MainWindow의 ViewModel - 사용자 입력 및 데이터 조회 처리
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private readonly BakingProductService _bakingProductService;

        // ObservableCollection으로 UI에 바인딩할 데이터 리스트
        public ObservableCollection<BakingProduct> BakingProducts { get; set; } = new ObservableCollection<BakingProduct>();

        // 검색 시작 날짜
        private DateTime _startDate = DateTime.Now.AddDays(-7);
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        // 검색 끝 날짜
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        // 배치번호 조건
        private string _batchNumber;
        public string BatchNumber
        {
            get => _batchNumber;
            set => SetProperty(ref _batchNumber, value);
        }

        // DelegateCommand - 버튼 클릭과 연결
        public DelegateCommand SearchCommand { get; }

        /// <summary>
        /// ViewModel 생성자 - BakingProductService 의존성 주입
        /// </summary>
        public MainWindowViewModel(BakingProductService bakingProductService)
        {
            _bakingProductService = bakingProductService;

            // 버튼 클릭 시 Search 메서드를 실행
            SearchCommand = new DelegateCommand(Search);
        }

        /// <summary>
        /// 검색 실행 메서드
        /// </summary>
        private void Search()
        {
            BakingProducts.Clear();
            var results = _bakingProductService.GetProductsByDateRangeAndBatch(StartDate, EndDate, BatchNumber);

            // 조회된 데이터를 ObservableCollection에 추가
            foreach (var product in results)
            {
                BakingProducts.Add(product);
            }
        }
    }
}
