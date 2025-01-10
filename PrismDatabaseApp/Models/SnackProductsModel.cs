using System;
using System.Windows;
using Microsoft.Win32;

namespace PrismDatabaseApp.Models
{
    /// <summary>
    /// BakingProducts 테이블과 매핑되는 모델 클래스
    /// </summary>
    public class SnackProductsModel
    {
        public int SnackId { get; set; }        // 제품 ID
        public string SnackName { get; set; }   // 제품 이름
        public DateTime CreatedDate { get; set; } // 생성 날짜
        public string BatchNumber { get; set; }   // 배치 번호
    }
}