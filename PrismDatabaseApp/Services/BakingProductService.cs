using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PrismDatabaseApp.Data;
using PrismDatabaseApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrismDatabaseApp.Services
{
    /// <summary>
    /// 데이터베이스 조회를 위한 서비스 클래스
    /// </summary>
    public class BakingProductService
    {
        private readonly AppDbContext _context;

        public BakingProductService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 날짜 범위와 배치번호에 따라 필터링된 데이터를 반환합니다.
        /// </summary>
        public List<BakingProduct> GetProductsByDateRangeAndBatch(DateTime startDate, DateTime endDate, string batchNumber)
        {
            // SQL 쿼리 생성
            StringBuilder query = new StringBuilder();
            query.Append("SELECT [ProductId], [ProductName], [CreatedDate], [BatchNumber] ");
            query.Append("FROM [BakingManagementDB].[dbo].[BakingProducts] ");
            query.Append("WHERE CAST([CreatedDate] AS DATE) BETWEEN @StartDate AND @EndDate ");

            // 배치번호 조건 추가
            if (!string.IsNullOrWhiteSpace(batchNumber))
            {
                query.Append("AND [BatchNumber] = @BatchNumber ");
            }

            // SQL 파라미터 추가
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@StartDate", startDate.Date),
                new SqlParameter("@EndDate", endDate.Date)
            };

            if (!string.IsNullOrWhiteSpace(batchNumber))
            {
                parameters.Add(new SqlParameter("@BatchNumber", batchNumber));
            }

            // 쿼리 실행
            return _context.BakingProducts
                           .FromSqlRaw(query.ToString(), parameters.ToArray())
                           .AsNoTracking()
                           .ToList();
        }
    }
}
