﻿using Magic.Guangdong.DbServices.Entities;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.Cert
{
    public class TemplateDto
    {
        public long? Id { get; set; } = YitIdHelper.NextId();

        public string ConfigJsonStrForImg { get; set; }

        public string CanvasJson { get; set; }

        public string ConfigJsonStrForPdf { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public string Remark { get; set; }

        public string Title { get; set; }

        public string? Url { get; set; } = "";

        public CertTemplateType Type { get; set; }

        public CertTemplateStatus Status { get; set; }

        public long ActivityId { get; set; } = 0;

        public CertTemplateLockStatus IsLock { get; set; }

    }

    [ExcelImporter(IsLabelingError = true)]
    public class ImportTemplateDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        [ImporterHeader(Name = "证书编号", Description = "按顺序递增即可,不可重复", IsAllowRepeat = false)]
        [Required(ErrorMessage = "证书不能为空")]
        public string CertNo { get; set; }

        /// <summary>
        /// 获奖人员
        /// </summary>
        [ImporterHeader(Name = "获奖者", Description = "获奖者可以是人名，也可以是队名")]
        [Required(ErrorMessage = "{0}不能为空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string AwardName { get; set; }

        /// <summary>
        /// ID号
        /// </summary>
        [ImporterHeader(Name = "ID号", Description = "一般为身份证号，准考证号等可以唯一确定身份的属性，不可重复")]
        [Required(ErrorMessage = "{0}不能为空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string IdNumber { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [ImporterHeader(Name = "自定义内容1", Description = "注意,如果证书模板中没有预留该自定义内容的位置，这里请留空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string CustomField1 { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [ImporterHeader(Name = "自定义内容2", Description = "注意,如果证书模板中没有预留该自定义内容的位置，这里请留空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string CustomField2 { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [ImporterHeader(Name = "自定义内容3", Description = "注意,如果证书模板中没有预留该自定义内容的位置，这里请留空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string CustomField3 { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [ImporterHeader(Name = "自定义内容4", Description = "注意,如果证书模板中没有预留该自定义内容的位置，这里请留空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string CustomField4 { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [ImporterHeader(Name = "自定义内容5", Description = "注意,如果证书模板中没有预留该自定义内容的位置，这里请留空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string CustomField5 { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [ImporterHeader(Name = "自定义内容6", Description = "注意,如果证书模板中没有预留该自定义内容的位置，这里请留空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string CustomField6 { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [ImporterHeader(Name = "自定义内容7", Description = "注意,如果证书模板中没有预留该自定义内容的位置，这里请留空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string CustomField7 { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [ImporterHeader(Name = "自定义内容8", Description = "注意,如果证书模板中没有预留该自定义内容的位置，这里请留空")]
        [MaxLength(50, ErrorMessage = "字数超出最大限制,请修改!")]
        public string CustomField8 { get; set; }
    }
}