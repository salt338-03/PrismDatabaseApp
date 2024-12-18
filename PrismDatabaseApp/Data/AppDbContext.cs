using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PrismDatabaseApp.Models;

namespace PrismDatabaseApp.Data
{
    /// <summary>
    /// Entity Framework Core의 DbContext 클래스
    /// </summary>
    public class AppDbContext : DbContext
    {
        public DbSet<BakingProduct> BakingProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQL Server 연결 문자열 설정
            optionsBuilder.UseSqlServer("Server=SUNJIN-NOTEBOOK\\MSSQLSERVERR;Database=BakingManagementDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BakingProducts 테이블 매핑
            modelBuilder.Entity<BakingProduct>(entity =>
            {
                entity.ToTable("BakingProducts");
                entity.HasKey(e => e.ProductId);
            });
        }
    }
}
