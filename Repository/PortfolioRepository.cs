using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository //인터페이스 구현
    {
        private readonly ApplicationDBContext _context; //EF Core DB 연결
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio); //Portfolio 테이블에 데이터 추가
            await _context.SaveChangesAsync(); //DB에 실제 반영 (INSERT 실행)
            return portfolio; //생성된 Portfolio 반환
        }

        public async Task<Portfolio> DeletePortfolio(AppUser appUser, string symbol)
        {
            //해당 사용자 + 해당 symbol에 해당하는 Portfolio(조인 데이터) 조회
            var portfolioModel = await _context.Portfolios.FirstOrDefaultAsync(x => x.AppUserId == appUser.Id && 
            x.Stock.Symbol.ToLower() == symbol.ToLower()); //사용자 조건&&stock 조건

            if(portfolioModel == null)
            {
                return null;
            }

            _context.Portfolios.Remove(portfolioModel); //Portfolio 제거 (조인 테이블 row 삭제)
            await _context.SaveChangesAsync(); //DB에 실제 반영 (DELETE 쿼리 실행)
            return portfolioModel; //삭제된 데이터 반환
        }

        public Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            //로그인한 사용자의 포트폴리오만 조회
            return _context.Portfolios.Where(u => u.AppUserId == user.Id)
            .Select(stock => new Stock
            {
               Id = stock.StockId,
               Symbol = stock.Stock.Symbol,
               CompanyName = stock.Stock.CompanyName,
               Purchase = stock.Stock.Purchase,
               LastDiv = stock.Stock.LastDiv,
               Industry = stock.Stock.Industry,
               MarketCap = stock.Stock.MarketCap 
            }).ToListAsync();
        }
    }
}