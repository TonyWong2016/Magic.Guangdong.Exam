﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Old
{
    public class ImportQuestionFromWord_old
    {
        public string title { get; set; }

        public string analysis { get; set; }

        public Guid typeId { get; set; }

        public Guid subjectId { get; set; }

        public double score { get; set; }

        public int questionType { get; set; }

        public string createdby { get; set; }

        public string degree { get; set; } = "normal";

        public string columnId { get; set; }

        public List<ImportQuestionItem> items { get; set; }
    }

    public class ImportQuestionItem_old
    {
        public string description { get; set; }

        public string code { get; set; }

        public string answer { get; set; }

        public int isAnswer { get; set; }

        public int index { get; set; } = 0;
    }
}
