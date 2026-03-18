using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class ClaimExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        //JWT에서 username 가져오기
        {
            //Token에서 Claims 정보를 가져옴
            return user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.GivenName)?
            .Value ?? string.Empty;
        }
    }
}