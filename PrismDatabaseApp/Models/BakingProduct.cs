using System;

namespace PrismDatabaseApp.Models
{
    /// <summary>
    /// BakingProducts 테이블과 매핑되는 모델 클래스
    /// </summary>
    public class BakingProduct
    {
        public int ProductId { get; set; }        // 제품 ID
        public string ProductName { get; set; }   // 제품 이름
        public DateTime CreatedDate { get; set; } // 생성 날짜
        public string BatchNumber { get; set; }   // 배치 번호
    }
}
