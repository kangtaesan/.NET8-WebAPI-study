using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Helpers;
using api.Models;


namespace api.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync(QueryObject query);
        Task<Stock?> GetByIdAsync(int id); //?는 FirstOrDefault 존재 때문에 사용
        //FirstOrDefault 의미는 조건에 맞는 첫번째 요소 반환 없으면 기본값 반환
        Task<Stock?> GetBySymbolAsync(string symbol); //symbol로 Stock 조회하는 기능 정의
        Task<Stock> CreateAsync(Stock stockModel);
        Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto);
        Task<Stock?> DeleteAsync(int id);
        Task<bool> StockExists(int id);
    }
}