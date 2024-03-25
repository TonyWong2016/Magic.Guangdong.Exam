using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Magic.Guangdong.Assistant.IService;

namespace Magic.Guangdong.Assistant
{
    public class JwtService:IJwtService
    {
        //const string apikey = "space_api";
        const string apiKey = "GD.exam";
        const string signKey = "cqmyg1sdssjtwmydtsgxqzygwcs!@#6";
        public string Make(string userName,string role,bool remember)
        {
            DateTime expires = DateTime.Now.AddHours(3);//3小时有效
            if (remember)
                expires = expires.AddDays(3);//3天有效

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, apiKey),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role),
                //new Claim("ExpiredTime", expires.ToString("yyyy/MM/dd HH:mm:ss"))
            };
            //string signkey = "cqmyg1sdssjtwmydtsgxqzygwcs!@#6";
            byte[] secBytes = Encoding.UTF8.GetBytes(Security.GenerateMD5Hash(signKey));
            var secKey = new SymmetricSecurityKey(secBytes);
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(claims: claims, expires: expires, signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            //RedisHelper.SetAsync("adminToken_" + userName, jwt, expires - DateTime.Now);
            return jwt;
        }

        public AccountClaim? Validate(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                return null;
            }
            AccountClaim accountClaim = new AccountClaim();
            //string secKey = "cqmyg1sdssjtwmydtsgxqzygwcs!@#6";
            JwtSecurityTokenHandler tokenHandler = new();

            TokenValidationParameters validationParameters = new TokenValidationParameters();
            var secrityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Security.GenerateMD5Hash(signKey)));
            validationParameters.IssuerSigningKey = secrityKey;
            validationParameters.ValidateIssuer = false;
            validationParameters.ValidateAudience = false;
            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken secToken);
                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"{claim.Type}={claim.Value}");
                    if (claim.Type.EndsWith("name"))
                    {
                        accountClaim.Name = claim.Value;
                        continue;
                    }
                    if(claim.Type.EndsWith("role"))
                    {
                        accountClaim.Role = claim.Value;
                        continue ;
                    }
                    if (claim.Type == "exp")
                    {
                        accountClaim.exp = Convert.ToInt64(claim.Value);
                        continue;
                    }
                }
                return accountClaim;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<bool> ValidateFilter(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                return false;
            }
            JwtSecurityTokenHandler tokenHandler = new();

            TokenValidationParameters validationParameters = new TokenValidationParameters();
            var secrityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Security.GenerateMD5Hash(signKey)));
            validationParameters.IssuerSigningKey = secrityKey;
            validationParameters.ValidateIssuer = false;
            validationParameters.ValidateAudience = false;
            try
            {
                var principal = await tokenHandler.ValidateTokenAsync(jwt, validationParameters);
                return principal.IsValid;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string Parse(string jwt)
        {
            string[] segments = jwt.Split(',');
            string head = JwtDecode(segments[0]);
            string payload = JwtDecode(segments[1]);
            return head + "--" + payload;
        }

        

        string JwtDecode(string s)
        {
            s = s.Replace("-", "+").Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2:
                    s += "==";
                    break;
                case 3:
                    s += "=";
                    break;

            }
            var bytes = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(bytes);
        }
    }

    public class AccountClaim
    {
        public string? Name { get; set; }

        public string? Role { get; set; }
        public long exp { get; set; }
    }
}
