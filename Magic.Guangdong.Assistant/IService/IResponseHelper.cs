using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.IService
{
    public interface IResponseHelper
    {
        string msg(string msg);

        int code(int code);

        dynamic data(dynamic data);

        dynamic ret(int code, string msg, dynamic data);

        dynamic ret(int code, string msg);

        dynamic ret();

        dynamic success(dynamic data, string msg = "success");

        dynamic ok(dynamic data, string msg = "success");
    }
}
