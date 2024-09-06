using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Interactive {

		[JsonProperty, Column(InsertValueSql = "getdate()")]
		public DateTime? Created_at { get; set; }

		[JsonProperty, Column(IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 该值指的是LogicID的类型，该表为用户在站点上互动的记录表，比如点赞，打星，评论等，因此logicid的值类型是多样的，目前根据type来判定
		/// </summary>
		[JsonProperty, Column(StringLength = 50)]
		public string LogicID { get; set; }

		/// <summary>
		/// 1-开头表示互动目标为课程，2-为课时，3-笔记，4-问答
		/// ，5-直播，11-课程收藏
		/// 12-课程推荐
		/// 13-课程评论
		/// 14-课程关注
		/// 21-课时评论
		/// 22-课时分享
		/// 31-笔记关注
		/// 32-笔记打星
		/// 33-笔记点赞
		/// 41-问答点赞，51-直播点赞，52-直播留言点赞，53-直播聊天点赞
		/// </summary>
		[JsonProperty]
		public int? Type { get; set; }

		[JsonProperty, Column(StringLength = 50)]
		public string UserID { get; set; }

		/// <summary>
		/// 具体数值，有则写入，无则为0
		/// </summary>
		[JsonProperty]
		public double? Val { get; set; } = 0d;

	}

}
