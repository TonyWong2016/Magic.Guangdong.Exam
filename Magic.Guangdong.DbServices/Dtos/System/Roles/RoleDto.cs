using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.System.Roles
{
    public class RoleDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();
        public string Name { get; set; }

        public string? Description { get; set; } = "无";

        //public Guid AdminId { get; set; }

        public long[]? PermissionIds
        {
            get
            {
                if (string.IsNullOrEmpty(PermissionIdsStr))
                {
                    return null;
                }
                string[] parts = PermissionIdsStr.Split(',');
                long[] longArray = new long[parts.Length];
                int index = 0;
                foreach (string str in parts)
                {
                    if (long.TryParse(str, out long number))
                    {
                        longArray[index++] = number;
                    }
                    else
                    {
                        // 处理转换失败的情况，可以选择抛出异常、替换默认值或跳过
                        // throw new ArgumentException($"无法将字符串'{str}'转换为long类型");
                    }
                }
                // 如果有未转换成功的项目，可以考虑截断数组或填充默认值
                Array.Resize(ref longArray, index); // 如果你想去掉未能转换的空位
                return longArray;
            }
        }

        public string PermissionIdsStr { get; set; } = "";

        public RoleType Type { get; set; }
    }

    public enum RoleType
    {
        Super = 1,
        Normal = 2,
        Other = 3
    }
}
