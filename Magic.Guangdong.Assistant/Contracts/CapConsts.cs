using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.Contracts
{
    public class CapConsts
    {
        public const string PREFIX = "guangdong.exam.service.";
        //public static string PREFIX = CheckInAssistant.ConfigurationHelper.GetSectionValue("SUBPREFIX");
        public static string GetCapMsgName(string name)
        {
            //string SUBPREFIX = CheckInAssistant.ConfigurationHelper.GetSectionValue("SUBPREFIX");
            return PREFIX + name;
        }

        
    }
}
