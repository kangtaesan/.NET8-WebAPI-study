using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    //EF Core에게 이 프로젝트 User 엔티티는 AppUser다 선언
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {
            
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //복합키(Composite Key) 설정
            builder.Entity<Portfolio>(x => x.HasKey(p => new {p.AppUserId, p.StockId}));

            //Portfolio ⇆ AppUser 관계 설정 코드
            builder.Entity<Portfolio>()
            .HasOne(u => u.AppUser) //AppUser 하나를 가진다
            .WithMany(u => u.portfolios) //여러 Portfolio를 가질 수 있다
            .HasForeignKey(p => p.AppUserId); //외래키는 AppUserId이다

            //Portfolio ⇆ Stock 관계 설정 코드
            builder.Entity<Portfolio>()
            .HasOne(u => u.Stock) //Stock 하나를 가진다
            .WithMany(u => u.portfolios) //여러 Portfolio를 가질 수 있다
            .HasForeignKey(p => p.StockId); //외래키는 StockId이다

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}