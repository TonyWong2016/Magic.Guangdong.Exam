﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dto.Menus
{
    public class MenuDto
    {
        public long  Id { get; set; }

        [DisplayName("菜单名称")]
        public string Name { get; set; }

        public int Depth { get; set; }

        public long ParentId { get; set; }
        [DisplayName("菜单功能")]
        public string Description { get; set; }

        public long PermissionId { get; set; }

        public string Router { get; set; }

        public Guid CreatorId { get; set; }

        public int Status { get; set; }

        public int IsLeef { get; set; }
    }
}