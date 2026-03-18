using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Stocks")]
    public class Stock
    {
        public int Id { get; set; }

        //string.Empty로 빈문자열이 낫지 null은 좋지않다.
        public string Symbol { get; set; } = string.Empty; 
        public string CompanyName { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]

        public decimal Purchase { get; set; } //decimal 소숫점
        [Column(TypeName = "decimal(18,2)")]

        public decimal LastDiv { get; set; }

        public string Industry { get; set; } = string.Empty;

        public long MarketCap { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<Portfolio> portfolios { get; set; } = new List<Portfolio>();
    }
}