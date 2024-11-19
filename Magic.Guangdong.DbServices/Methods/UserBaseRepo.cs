using Magic.Guangdong.Assistant.Dto;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class UserBaseRepo : ExaminationRepository<UserBase>, IUserBaseRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public UserBaseRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<bool> InsertUserBaseSecurity(UserBase user)
        {
            if (user == null)
            {
                return false;
            }
            var maskIdCard = new MaskDataDto();
            user.IsSecurity = 0;
            maskIdCard.keyId = user.KeyId;
            maskIdCard.keySecret = user.KeySecret;
            if (string.IsNullOrEmpty(user.IdCard)) 
            { return true; } 
            maskIdCard.text = user.IdCard.Trim();
            if (user.CardType == 0)
            {                
                maskIdCard.maskDataType = MaskDataType.ChinaIdCard;
            }
            else
            {
                maskIdCard.maskDataType = MaskDataType.Other;
            }
            if (maskIdCard.valid)
            {
                user.PrefixIdcard = maskIdCard.splitTexts[0];
                user.SuffixIdcard = maskIdCard.splitTexts[1];
                user.IdCard = maskIdCard.encryptText;
                user.HashIdcard = maskIdCard.hashText;
                user.IsSecurity = 1;
            }
            else
            {
                return false;
            }
            var maskPhone = new MaskDataDto();
            maskPhone.keyId = user.KeyId;
            maskPhone.keySecret = user.KeySecret;

            
            if (string.IsNullOrEmpty(user.Mobile))
            { return true; }
            maskPhone.text = user.Mobile.Trim();
            if (user.CardType == 0)
            {
                maskPhone.maskDataType = MaskDataType.ChinaCellPhone;
            }
            else
            {
                maskPhone.maskDataType = MaskDataType.Other;
            }

            if (maskPhone.valid)
            {
                user.PrefixMobile = maskPhone.splitTexts[0];
                user.SuffixMobile = maskPhone.splitTexts[1];
                user.Mobile = maskPhone.encryptText;
                user.HashMobile = maskPhone.hashText;
                user.IsSecurity = 1;
            }
            else
            {
                return false;
            }
            
            return await fsql.Get(conn_str).Insert(user).ExecuteAffrowsAsync() == 1;
        }
    }
}
