using FreeSql.Internal;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.Monitor
{
    public class MainMonitorDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();

        public string Title { get; set; }

        public string Description { get; set; }

        public List<MonitorStreamConfigDto> MonitorStreamConfigDtos { get; set; }
    }

    public class MonitorStreamConfigDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();

        public string StreamSessionId { get; set; } = Assistant.Utils.GenerateRandomCodeFast(6,3);

        public string StreamName { get; set; }

        public StreamType StreamType { get; set; } = StreamType.Camera;

        public StreamInputType InputType { get; set;} = StreamInputType.AudioVideo;

        public string InputJson { get; set; }

    }
}
