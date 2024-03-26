using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.IService
{
    public interface IJwtService
    {
        string Make(string userId, string userName, bool remember);

        string Make(string userId, string userName, DateTime expires);

        AccountClaim? Validate(string jwt);
    }
}
