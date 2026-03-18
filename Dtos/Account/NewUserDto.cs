using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Account
{
    public class NewUserDto
    //회원가입 결과를 클라이언트에게 보내기 위한 DTO, 생성 때문이 아니라 "응답(Response) 구조" 때문
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}