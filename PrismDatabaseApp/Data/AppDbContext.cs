using Microsoft.EntityFrameworkCore;
using PrismDatabaseApp.Models;

namespace PrismDatabaseApp.Data
{
    /// <summary>
    /// Entity Framework Core의 DbContext 클래스
    /// </summary>
    public class AppDbContext : DbContext
    {
        public DbSet<BakingProductModel> BakingProducts { get; set; }

        //AppDbContext 생성자는 Entity Framework Core가 데이터베이스 연결 정보를 초기화하도록 DbContextOptions를 전달받기 위해 필요합니다.
        //DbContextOptions는 Entity Framework Core에서 데이터베이스 연결 정보와 구성 설정(SQL Server, SQLite 등)을 관리하는 객체입니다.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BakingProducts 테이블 매핑
            modelBuilder.Entity<BakingProductModel>(entity =>
            {
                entity.ToTable("BakingProducts");
                entity.HasKey(e => e.ProductId);
            });
        }
    }
}
//코드는결국 db 테이블이랑 c# 이랑 매핑하는것, 그이유는 엔티티프레임워크 코어를 사용해서 c# 에서 자동으로 쿼리를 생성하거나 링큐문으로 조작하려고