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
        public string Make(string userId,string userName,bool remember)
        {
            DateTime expires = DateTime.Now.AddHours(3);//3小时有效
            if (remember)
                expires = expires.AddDays(3);//3天有效

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, apiKey),
                new Claim(ClaimTypes.Sid, userId),
                new Claim(ClaimTypes.Name, userName),
                //new Claim(ClaimTypes.Version,Utils.DateTimeToTimeStamp(expires).ToString())
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

        public string Make(string userId, string userName, DateTime expires)
        {
            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, apiKey),
                new Claim(ClaimTypes.Sid, userId),
                new Claim(ClaimTypes.Name, userName),
            };
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
                    if(claim.Type.EndsWith("sid"))
                    {
                        accountClaim.Sid = claim.Value;
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

        public static async Task<JwtSecurityToken> DecodeJwtAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("your-secret-key"); // 对于HS256算法
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://pews7koru675-demo.authing.cn/oidc",
                ValidateAudience = true,
                ValidAudience = "64a389fbb0c0a11e43437345",
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
            };

            try
            {
                var validationResult = await tokenHandler.ValidateTokenAsync(token, validationParameters);
                JwtSecurityToken validatedToken = validationResult.SecurityToken as JwtSecurityToken;
                if (validationResult.IsValid && validatedToken != null)
                {
                    return validatedToken;
                }
                else
                {
                    Console.WriteLine($"Token validation failed: {validationResult.Exception.Message}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // 处理验证过程中的其他异常
                Console.WriteLine($"Unexpected error during token validation: {ex.Message}");
                return null;
            }
        }
        
        //临时方案
        public static string DecodeJwtString(string jwtToken)
        {
            // 分割JWT的三个部分：头部（header）、载荷（payload）和签名（signature）
            var parts = jwtToken.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid JWT format. Expected three parts separated by dots.");
            }

            // 解码载荷（第二个部分）
            string payloadBase64Url = parts[1];
            byte[] payloadBytes = Base64UrlEncoder.DecodeBytes(payloadBase64Url);
            string decodedPayloadJson = Encoding.UTF8.GetString(payloadBytes);

            // 输出解码后的载荷
            Console.WriteLine("Decoded Payload JSON:");
            Console.WriteLine(decodedPayloadJson);

            // 可选：将解码后的JSON字符串反序列化为强类型对象
            var payload = JsonHelper.JsonDeserialize<Dictionary<string, object>>(decodedPayloadJson);
            Console.WriteLine("\nDecoded Payload as Dictionary:");
            string sub = "";
            long exp = 0;
            foreach (var entry in payload)
            {
                if(entry.Key.ToLower().Contains("sub"))
                    sub = entry.Value.ToString();
                if (entry.Key.ToLower().Contains("exp"))
                {
                    exp = Convert.ToInt64(entry.Value);
                }                    
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }
            if(Utils.TimeStampToDateTime(exp) < DateTime.Now)
            {
                return "expired";
            }
            return sub;
        }
        public static bool IsJwtTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);

                // 获取过期时间（Unix时间戳，即自1970年1月1日以来的秒数）
                var expiration = long.Parse(jwtToken.Claims.First(claim => claim.Type == "exp").Value);

                // 转换为DateTime对象
                var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiration);

                // 检查是否已过期
                return expiresAt < DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                // 如果解析或提取过期时间时出错，认为token无效
                Console.WriteLine($"Failed to validate token expiration: {ex.Message}");
                return true;
            }
        }

        public static AccountClaim? ValidateJwt(string jwt)
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
                    //Console.WriteLine($"{claim.Type}={claim.Value}");
                    if (claim.Type.EndsWith("name"))
                    {
                        accountClaim.Name = claim.Value;
                        continue;
                    }
                    if (claim.Type.EndsWith("sid"))
                    {
                        accountClaim.Sid = claim.Value;
                        continue;
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

        public string? Sid { get; set; }
        public long exp { get; set; }
    }
}
