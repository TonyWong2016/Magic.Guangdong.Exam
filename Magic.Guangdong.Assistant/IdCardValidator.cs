using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    public class IdCardValidator
    {
        // 地区代码表
        private static readonly HashSet<string> AreaCodes = new HashSet<string>
        {
            "11", "12", "13", "14", "15", "21", "22", "23", "31", "32", "33",
            "34", "35", "36", "37", "41", "42", "43", "44", "45", "46", "50",
            "51", "52", "53", "54", "61", "62", "63", "64", "65", "71", "81",
            "82", "91"
        };

        private const string ValidCheckDigits = "10X98765432";

        // 权重数组
        private static readonly int[] Weights = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
        public static bool IsValidIdCard(string idCard)
        {
            if (idCard.Length != 18)
                return false;

            // 检查地区代码
            if (!AreaCodes.Contains(idCard.Substring(0, 2)))
                return false;

            // 检查生日
            if (!IsValidDate(idCard.Substring(6, 8)))
                return false;

            // 检查校验码
            return IsCorrectCheckDigit(idCard);
        }

        private static bool IsValidDate(string dateStr)
        {
            DateTime date;
            return DateTime.TryParseExact(dateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out date);
        }

        private static bool IsCorrectCheckDigit(string idCard)
        {
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += (idCard[i] - '0') * Weights[i];
            }

            char checkDigit = ValidCheckDigits[sum % 11];
            return checkDigit == idCard[17];
        }
    }
}
