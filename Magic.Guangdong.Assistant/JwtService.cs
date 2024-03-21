using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    internal class JwtService
    {
        public string Make(string apikey = "space_api")
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, apikey));
            claims.Add(new Claim(ClaimTypes.Name, apikey));
            claims.Add(new Claim(ClaimTypes.Role, "User"));
            claims.Add(new Claim("Sender", "Tony"));
            DateTime expires = DateTime.Now.AddDays(1);
            string signkey = "cqmyg1sdssjtwmydtsgxqzygwcs!@#6";
            byte[] secBytes = Encoding.UTF8.GetBytes(signkey);
            var secKey = new SymmetricSecurityKey(secBytes);
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(claims: claims, expires: expires, signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return jwt;
        }

        public bool Validate(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                return false;
            }
            string secKey = "cqmyg1sdssjtwmydtsgxqzygwcs!@#6";
            JwtSecurityTokenHandler tokenHandler = new();

            TokenValidationParameters validationParameters = new TokenValidationParameters();
            var secrityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            validationParameters.IssuerSigningKey = secrityKey;
            validationParameters.ValidateIssuer = false;
            validationParameters.ValidateAudience = false;
            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken secToken);
                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"{claim.Type}={claim.Value}");
                }
                return true;
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
}
