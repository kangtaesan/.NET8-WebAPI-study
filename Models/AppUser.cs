using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    //IdentityUser는 AspNetUsers 테이블을 사용 중
    //IdentityUser는 사용자 인증에 필요한 기본 사용자 모델 제공
    public class AppUser : IdentityUser
    {
        public List<Portfolio> portfolios { get; set; } = new List<Portfolio>();
    }
}