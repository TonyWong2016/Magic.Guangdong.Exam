﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.Contracts
{
    public static class UtilConsts
    {
        public const string CLIENTFACTORYNAME = "MagicHttpClient";

        /// <summary>
        /// 它表示通道（Channel）可以容纳的最大元素数量
        /// </summary>
        public const int SSECapacity = 20;

        /// <summary>
        /// 最多保留60天的记录，这是上限
        /// </summary>
        public const int DaysOldToDeleteLimit = 60;
    }
}
