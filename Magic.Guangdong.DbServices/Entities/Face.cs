using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Entities
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Face {

		[JsonProperty, Column(IsPrimary = true)]
		public long Id { get; set; } = YitIdHelper.NextId();

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime createdAt { get; set; }

		/// <summary>
		/// 人脸信息（json字符串）
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(500)")]
		public string faceInfo { get; set; }

		/// <summary>
		/// 人脸标识
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string faceToken { get; set; }

		/// <summary>
		/// 组别，{角色}_{活动名称}_{年份/年月}
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)")]
		public string groupId { get; set; }

		/// <summary>
		/// 角色组，user-用户，admin-管理员
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(20)")]
		public string role { get; set; } = "’user‘";

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime updatedAt { get; set; }

		/// <summary>
		/// 用户id
		/// </summary>
		[JsonProperty, Column(DbType = "varchar(50)", IsNullable = false)]
		public string userId { get; set; } = "0";

	}

}
