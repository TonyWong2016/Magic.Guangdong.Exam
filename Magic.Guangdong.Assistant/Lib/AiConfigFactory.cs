using Magic.Guangdong.Assistant.CloudModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.Lib
{
    public class AiConfigFactory
    {
        private readonly List<AiConfig> _aiConfigs;

        public AiConfigFactory(List<AiConfig> aiConfigs)
        {
            _aiConfigs = aiConfigs;
        }

        public AiConfig GetConfigByModel(string Model)
        {
            return _aiConfigs.FirstOrDefault(config => config.Model.Equals(Model, StringComparison.OrdinalIgnoreCase));
        }

        public List<AiConfig> GetConfigs()
        {
            return _aiConfigs;
        }
    }
}
