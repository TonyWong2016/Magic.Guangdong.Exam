using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.Contracts
{
    public class CapConsts
    {
        public const string PREFIX = "exam.service.";

        public const string ClientPrefix = "exam.service.client.";

        public const string TeacherPrefix = "exam.service.teacher.";

        public const string MsgIdCacheOaName = "capExamOaMsgs";

        public const string MsgIdCacheClientName = "capExamClientMsgs";

        public const string MsgIdCacheTeacherName = "capExamTeacherMsgs";

        public const int CapMsgMaxLength = 2000;
        //public static string PREFIX = CheckInAssistant.ConfigurationHelper.GetSectionValue("SUBPREFIX");
        public static string GetCapMsgName(string name)
        {
            string SUBPREFIX = ConfigurationHelper.GetSectionValue("CAPSuffix");
            return PREFIX + name+ SUBPREFIX;
        }
        
    }
}
