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
    public class SnackProductsService
    {
        private readonly AppDbContext _context;

        public SnackProductsService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "AppDbContext cannot be null.");
        }

        /// <summary>
        /// 날짜 범위와 배치번호에 따라 필터링된 데이터를 반환합니다.
        /// </summary>
        public List<SnackProductsModel> GetProductsByDateRangeAndBatch(DateTime startDate, DateTime endDate, string batchNumber)
        {
            // SQL 쿼리 생성
            StringBuilder query = new StringBuilder();
            query.Append("SELECT [SnackId], [SnackName], [CreatedDate], [BatchNumber] ");
            query.Append("FROM [BakingManagementDB].[dbo].[SnackProducts] ");
            query.Append("WHERE CAST([CreatedDate] AS DATE) BETWEEN @StartDate AND @EndDate ");

            // 배치번호 조건 추가
            if (!string.IsNullOrWhiteSpace(batchNumber))
            {
                query.Append("AND [BatchNumber] = @BatchNumber ");
            }

            // 쿼리 실행
            return _context.SnackProducts
                           .FromSqlRaw(query.ToString(), new object[]
                           {
                               new Microsoft.Data.SqlClient.SqlParameter("@StartDate", startDate.Date),
                               new Microsoft.Data.SqlClient.SqlParameter("@EndDate", endDate.Date),
                               new Microsoft.Data.SqlClient.SqlParameter("@BatchNumber", batchNumber ?? string.Empty)
                           })
                           .AsNoTracking()
                           .ToList();
        }
    }
}
