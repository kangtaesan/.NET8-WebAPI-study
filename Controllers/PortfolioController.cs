using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        public PortfolioController(UserManager<AppUser> userManager,
        IStockRepository stockRepo, IPortfolioRepository portfolioRepo)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _portfolioRepo = portfolioRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername(); //ClaimExtensions에서 가져옴
            var appUser = await _userManager.FindByNameAsync(username); //DB에서 사용자 찾기
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser!); //인터페이스 호출
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername(); //현재 로그인한 사용자 username 추출 (JWT Claim에서 가져옴)
            var appUser = await _userManager.FindByNameAsync(username); //username 기반으로 실제 DB 사용자 객체 조회
            var stock = await _stockRepo.GetBySymbolAsync(symbol); //전달받은 symbol로 주식(Stock) 조회

            if(stock== null) return BadRequest("Stock not found"); //해당 주식이 존재하지 않으면 에러 반환

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser); //현재 사용자의 포트폴리오 목록 조회

            if(userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower())) 
            return BadRequest("Cannot add same stock to portfolio"); //이미 같은 symbol이 포트폴리오에 있는지 체크(중복 체크)

            var portfolioModel = new Portfolio
            {
                StockId = stock.Id, //Stock 테이블 FK
                AppUserId = appUser.Id //User 테이블 FK
            }; //Portfolio 객체 생성 (조인 테이블 역할)

            await _portfolioRepo.CreateAsync(portfolioModel); //DB에 Portfolio 저장

            if(portfolioModel == null)
            {
                return StatusCode(500, "Could not create"); //저장 실패 시 에러 반환
            }
            else
            {
                return Created(); //성공 응답 반환
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username = User.GetUsername(); //JWT에서 현재 로그인한 username 추출
            var appUser = await _userManager.FindByNameAsync(username); //DB에서 실제 사용자(AppUser) 조회

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser); //해당 사용자의 포트폴리오 목록 조회

            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();
            //symbol이 일치하는 stock만 필터링 (대소문자 무시)

            if(filteredStock.Count() == 1) //정확히 1개 존재하면 삭제 진행
            {
                await _portfolioRepo.DeletePortfolio(appUser, symbol); //Repository로 삭제 요청 (DB 처리 위임)
            }
            else
            {
                return BadRequest("Stock not in ouyr portfolio"); //해당 stock이 없으면 에러 반환
            }

            return Ok();
        }
    }
}