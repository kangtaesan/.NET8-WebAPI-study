using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager; //사용자 조회
        private readonly ITokenService _tokenService; //JWT 생성
        private readonly SignInManager<AppUser> _signInManager; //비밀번호 검증
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService
        , SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

            //DB에서 username이 일치하는 사용자 찾기
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            //사용자 존재 여부 확인
            if(user == null) return Unauthorized("Invaild username!");

            //비밀번호 검증
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            //성공하면 result.Succeeded = true, 실패하면 false
            if(!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");

            return Ok(
                //로그인 성공시 응답 DTO 생성
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user) //JWT 생성  
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if(!ModelState.IsValid)
                return BadRequest(ModelState);

                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password!);

                if(createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if(roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                Token = _tokenService.CreateToken(appUser)
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                    {
                        return StatusCode(500, createdUser.Errors);
                    }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}