﻿using Magic.Guangdong.DbServices.Entities;

namespace Magic.Guangdong.DbServices.Dtos.Report.ReportInfo
{
    public class ReportInfoDto
    {
        public long Id { get; set; }
        public string AccountId { get; set; }

        public long ActivityId { get; set; }

        public int ProvinceId { get; set; }

        public int CityId { get; set; }

        public int? DistrictId { get; set; } = 0;

        public string Email { get; set; } = "";

        public string Mobile { get; set; }= "";

        public Guid ExamId { get; set; } = Guid.Empty;

        public string IdCard { get; set; } = "";

        public string Job { get; set; } = "";

        public string Name { get; set; } = "";

        public string OtherInfo { get; set; } = "";

        public string Address { get; set; } = "";

        public long UnitId { get; set; }

        public CardType CardType { get; set; } = CardType.China;

        /// <summary>
		/// 联系方式可用程度
		/// 0-未验证
		/// 1-邮箱验证可用
		/// 2-手机号可用
		/// 3-邮箱和手机号都可用
		/// </summary>
        public int ConnAvailable { get; set; }

        /// <summary>
        /// 生成一个订单号返回到前台
        /// </summary>
        public string OrderTradeNumber { get; set; }

        public string Photo { get; set; }

        public string ReportNumber { get; set; }

        public long TagId { get; set; }
    }

    public class GetReportListDto
    {
        public long? ActivityId { get; set; }

        public string AccountId { get; set; }

        public Guid? ExamId { get; set; }

        public Guid? OrderId { get; set; }

        public int? ReportStatus { get; set; }

        public int pageIndex {  get; set; }

        public int pageSize { get; set; }
    }

    public class ReportInfoApiDto
    {
        public bool isValid
        {
            get
            {
                if (string.IsNullOrEmpty(AccountId)
                    || string.IsNullOrEmpty(Name)
                    || string.IsNullOrEmpty(IdCard)
                    || string.IsNullOrEmpty(Mobile)
                    || string.IsNullOrEmpty(Email))
                    return false;
                if (CardType == CardType.China 
                    && 
                    (Mobile.Length > 11 || !Assistant.IdCardValidator.IsValidIdCard(IdCard))
                    )
                    return false;

                return true;
            }
        }
        public long Id { get; set; }
        public string AccountId { get; set; }

        public long ActivityId { get; set; }

        public int ProvinceId { get; set; }

        public int CityId { get; set; }

        public int? DistrictId { get; set; } = 0;

        public string Email { get; set; } = "";

        public string Mobile { get; set; } = "";

        public string HashMobile
        {
            get
            {
                if (string.IsNullOrEmpty(Mobile))
                    return "";
                return Assistant.Security.GenerateMD5Hash(Mobile);
            }
        }

        public Guid ExamId { get; set; } = Guid.Empty;

        public string IdCard { get; set; } = "";

        public string HashIdCard
        {
            get
            {
                if (string.IsNullOrEmpty(IdCard))
                    return "";
                return Assistant.Security.GenerateMD5Hash(IdCard);
            }
        }

        public string Job { get; set; } = "";

        public string Name { get; set; } = "";

        public string OtherInfo { get; set; } = "";

        public string Address { get; set; } = "";

        public long UnitId { get; set; }

        public CardType CardType { get; set; } = CardType.China;

        /// <summary>
		/// 联系方式可用程度
		/// 0-未验证
		/// 1-邮箱验证可用
		/// 2-手机号可用
		/// 3-邮箱和手机号都可用
		/// </summary>
        public int ConnAvailable { get; set; }

        /// <summary>
        /// 生成一个订单号返回到前台
        /// </summary>
        public string OrderTradeNumber { get; set; }

        public string Photo { get; set; }

        public string ReportNumber { get; set; }

        public string Tag { get; set; }
        /// <summary>
        /// 标签ID，如果该id存在，则优先使用TagId
        /// </summary>
        public long TagId { get; set; } = 0;

        public ReportAttributeParam reportAttributes { get; set; }
    }

    public class ReportAttributeParam
    {
        public long ReportId { get; set; } = 0;

        public string Participants { get; set; }

        public string Teachers { get; set; }

        public string Schools { get; set; }

        public string TeamName { get; set; }

        public string ProjectNo { get; set; }

        public string GroupName { get; set; }

        public string Other { get; set; }
    }

    public class ReturnVerifyResult
    {
        public long reportId { get; set; }

        public long recordId { get; set; }

        public ExamComplated complated { get; set; } = ExamComplated.No;

        public int verifyCode { get; set; } = 0;

        public string verifyMsg { get; set; }

        public VerifyReportInfo verifyReportInfo { get; set; }
    }

    public class VerifyReportInfo
    {
        public string name { get; set;  }

        public string secruityIdCard { get; set;  }

        public string mobile { get; set; }

        public string email { get; set; } 

        public string reportNumber { get; set; }
    }
}
