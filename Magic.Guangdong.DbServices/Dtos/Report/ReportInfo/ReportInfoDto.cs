﻿using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Report.ReportInfo
{
    public class ReportInfoDto
    {
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
    }
}