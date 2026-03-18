using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface IPortfolioRepository
    {
        //사용자의 포트폴리오 주식 목록 조회
        Task<List<Stock>> GetUserPortfolio(AppUser user);
        Task<Portfolio> CreateAsync(Portfolio portfolio); //Portfolio 생성 (조인 테이블 데이터 추가)
        Task<Portfolio> DeletePortfolio(AppUser appUser, string symbol); //특정 사용자의 특정 stock 제거 (조인 테이블 delete)
    }
}