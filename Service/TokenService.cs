using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config; //설정 파일(appsettings.json) 읽기용 객체
        private readonly SymmetricSecurityKey _key; //JWT 서명에 사용할 비밀키 객체

        //IConfiguration은 appsettings.json 같은 설정 파일을 읽기 위한 인터페이스
        public TokenService(IConfiguration config)
        //왜 생성자에서 _key를 미리 만드냐면 서비스가 만들어질 때 한 번 준비해두면 됨
        //CreateToken()은 토큰 생성 로직에만 집중, 즉 초기화 작업을 생성자에서 분리한 것
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
            //SigningKey는 문자열인데, 암호화/서명 라이브러리는 바이트 배열 형태를 원함
            //new SymmetricSecurityKey(...) 이건 변환한 바이트 배열을 JWT 서명용 키 객체로 만드는 것
            //여기서 Symmetric은 같은 키로 서명하고 검증하는 방식이라는 뜻
        }
        public string CreateToken(AppUser user)
        //이 메서드는 사용자 정보를 받아 JWT 문자열을 만들어 반환
        {
            //토큰 안에 넣을 사용자 정보 목록 
            //JWT는 단순히 “로그인 성공”만 담는 게 아니라 사용자 관련 정보를 payload 안에 담을 수 있다.
            var claims = new List<Claim>
            {
              new Claim(JwtRegisteredClaimNames.Email, user.Email!), //의미: 이 토큰의 사용자 이메일은 user.Email
              new Claim(JwtRegisteredClaimNames.GivenName, user.UserName!)// 의미: 이 토큰의 사용자 이름은 user.UserName  
            };

            //“어떤 키로, 어떤 알고리즘으로 서명할지” 정하는 부분
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            //SecurityAlgorithms.HmacSha512Signature는 서명 방식으로 HMAC-SHA512 알고리즘으로 서명하겠다라는 뜻
            //SigningCredentials은 토큰이 진짜 우리 서버가 만든 것인지중간에 내용이 바뀌지 않았는지 확인하는 JWT 위조 방지 설정

            //JWT 설계도 - JWT를 만들기 전 “이 토큰에 어떤 내용을 넣고 어떤 조건으로 만들지” 한 번에 모아둔 객체
            var tokenDescriptor = new SecurityTokenDescriptor
            {
              Subject = new ClaimsIdentity(claims), //아까 만든 claims 목록을 토큰의 주체(subject) 정보로 넣는 것
              //ClaimsIdentity - 여러 개의 Claim을 하나의 사용자 신원 정보처럼 묶는 역할
              Expires = DateTime.Now.AddDays(7), //토큰 만료 시간
              SigningCredentials = creds, //아까 만든 서명 설정을 실제 토큰 생성에 적용하는 부분
              Issuer = _config["JWT:Issuer"], //토큰의 발급자를 넣는 부분
              Audience = _config["JWT:Audience"] //토큰의 사용 대상자를 넣는 부분
              //Issuer / Audience를 넣는 이유는 발급자가 우리 서버가 맞는지/이 토큰이 우리 서비스용이 맞는지 확인하기 위해서
              //단순한 문자열이 아니라 검증 기준값이 되는 것
            };

            //이 객체는 실제로 JWT를 만들어주고 문자열로 바꿔주는 도구
            var tokenHandler = new JwtSecurityTokenHandler();

            //만든 설계도(tokenDescriptor)를 바탕으로 실제 JWT 객체를 만듬
            //단, 이 시점의 token은 아직 객체 형태
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //여기서 CreateToken는 .NET JWT 라이브러리 메서드

            //JWT 객체를 실제로 클라이언트에게 전달할 수 있는 문자열 형태로 변환
            return tokenHandler.WriteToken(token);
        }
    }
}