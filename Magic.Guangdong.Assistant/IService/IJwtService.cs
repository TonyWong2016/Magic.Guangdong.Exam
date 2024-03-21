using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.IService
{
    public interface IJwtService
    {
        string Make(string userName, string role, bool remember);

        AccountClaim? Validate(string jwt);
    }
}
