﻿using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace Magic.Cxxy.DbServices {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class Proc_CourseLessonShare {

		[JsonProperty, Column(Name = "@CourseID")]
		public Guid CourseID { get; set; }

		[JsonProperty, Column(Name = "@LessionID")]
		public Guid LessionID { get; set; }

	}

}