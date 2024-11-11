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
	public partial class TagRelations {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
		public string AssociationId { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [JsonProperty]
		public int IsDeleted { get; set; } = 0;

		[JsonProperty, Column(DbType = "varchar(50)")]
		public string OriginalId { get; set; }

		[JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
		public string TableName { get; set; }

        [JsonProperty, Column(DbType = "varchar(100)", IsNullable = false)]
        public string HashRelation { get; set; }

        [JsonProperty]
		public long TagId { get; set; }

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    }

}
