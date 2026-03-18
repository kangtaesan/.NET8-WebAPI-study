using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = string.Empty;
        public int? StockId { get; set; }

        //public Stock? Stock { get; set; } //navigation property는 제외
        //해당 Comment가 어떤 Stock에 속해있는지 알려주는 부분
    }
}