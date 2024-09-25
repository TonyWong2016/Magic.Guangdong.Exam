using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.Contracts
{
    public class MinioSettings
    {
        public required string Endpoint { get; set; }
        public required string AccessKey { get; set; }
        public required string SecretKey { get; set; }

        public required bool UseSSL { get; set; } = true;

        //public bool SSL 
        //{ 
        //    get
        //    {
        //        return UseSSL.ToLower() == "yes";
        //    }
        //}
    }
}
