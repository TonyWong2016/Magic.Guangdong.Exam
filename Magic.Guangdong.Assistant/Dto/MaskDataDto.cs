using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.Dto
{
    public class MaskDataDto()
    {
        public string text { get; set; }

        public MaskDataType maskDataType { get; set; }

        public uint firstPoint
        {
            get
            {
                if (maskDataType == MaskDataType.ChinaCellPhone)
                    return 3;
                if (maskDataType == MaskDataType.ChinaIdCard)
                    return 4;
                return Convert.ToUInt32(Math.Ceiling(Convert.ToDouble(text.Length) / 3));
            }
            set { }
        }

        public uint lastPoint {
            get
            {
                if (maskDataType == MaskDataType.ChinaCellPhone)
                    return 7;
                if (maskDataType == MaskDataType.ChinaIdCard)
                    return 14;
                return Convert.ToUInt32(Math.Ceiling(Convert.ToDouble(text.Length) / 1.5));
            }
            set { }
        }

        public string keyId { get; set; }

        public string keySecret { get; set; }

        public bool valid
        {
            get
            {
                if (string.IsNullOrEmpty(keyId) || string.IsNullOrEmpty(keySecret))
                    return false;

                if (maskDataType == MaskDataType.ChinaCellPhone && text.Length != 11)
                    return false;

                if (maskDataType == MaskDataType.ChinaIdCard && !Assistant.IdCardValidator.IsValidIdCard(text))
                    return false;

                if (firstPoint>=text.Length || firstPoint>lastPoint || lastPoint>=text.Length || lastPoint==0)
                    return false;

                return true;
            }
        }

        public string encryptText
        {
            get
            {
                //if (!valid)
                //    return "error data";
                //解码的时候，要先转回普通字符，在解码
                return Assistant.Utils.ToBase64Str(Assistant.Security.Encrypt(text, Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret)));
            }
            //get;set;
        }
       // public string[]? splitTexts { get; set; }
        public string[]? splitTexts
        {
            get
            {
                var textParts = new List<string>()
                    {
                        text.Substring(0, (int)firstPoint),
                        text.Substring((int)lastPoint, text.Length - (int)lastPoint)
                    };
                return textParts.ToArray();
            }
        }

        public string hashText
        {
            get
            {
                return Security.GenerateMD5Hash(text);
            }
        }

    }

    public enum MaskDataType
    {
        ChinaIdCard,
        ChinaCellPhone,
        Other
    }
}
