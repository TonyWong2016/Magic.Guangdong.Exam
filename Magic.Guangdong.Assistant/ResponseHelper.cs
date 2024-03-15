using Magic.Guangdong.Assistant.IService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    public class ResponseHelper : IResponseHelper
    {
        protected string _message = "";

        protected int _code = 0;

        protected dynamic _data;
        /// <summary>
        /// 返回代码. 0-失败，1-成功，其他-具体见方法返回值说明
        /// </summary>        
        public int code(int code)
        {
            this._code = code;
            return _code;
        }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string msg(string msg)
        {
            _message = msg;
            Logger.Info(msg);
            if (_code < 0)
            {
                Logger.Error(_message);
            }
            return _message;
        }

        /// <summary>
        /// 总条数（记录实际返回的记录条数）
        /// </summary>
        //public int count { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public dynamic data(dynamic data)
        {
            _data = data;
            return _data;
        }

        public ResponseHelper()
        {
            _code = 0;
        }

        public dynamic ret(int code, string msg, dynamic data)
        {
            _code = code;
            _message = msg;
            if (_code < 0)
            {
                Logger.Error(_message);
            }
            _data = data;
            return new { code = _code, msg = _message, data = _data };
        }

        public dynamic ret(int code, string msg)
        {
            _code = code;
            _message = msg;
            if (_code < 0)
            {
                Logger.Error(_message);
            }
            return new { code = _code, msg = _message };
        }

        public dynamic ret()
        {
            return new { code = _code, msg = _message, data = _data };
        }

        public dynamic success(dynamic data, string msg = "success")
        {
            _message = msg;
            _data = data;
            return new { code = 1, msg = _message, data = _data };
        }

        public dynamic ok(dynamic data, string msg = "success")
        {
            _message = msg;
            _data = data;
            return new { code = 0, msg = _message, data = _data };
        }
    }
}
