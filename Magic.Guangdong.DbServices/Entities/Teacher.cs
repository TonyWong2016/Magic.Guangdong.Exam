using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Teacher {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty, Column(DbType = "varchar(50)")]
		public string CreatedBy { get; set; } 

		[JsonProperty, Column(StringLength = 500)]
		public string Intro { get; set; }

		[JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Name { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonProperty, Column(StringLength = 50, IsNullable = false)]
        public string TeachNo { get; set; }

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Email { get; set; } = string.Empty;

		[JsonProperty, Column(StringLength = 50, IsNullable = false)]
		public string Mobile { get; set; } = string.Empty;

        [JsonProperty]
        public string Password { get; set; }

        [JsonProperty]
        public string KeySecret { get; set; } = Assistant.Utils.GenerateRandomCodePro(16);

        [JsonProperty]
        public string KeyId { get; set; } = Assistant.Utils.GenerateRandomCodePro(16);


    }

}
