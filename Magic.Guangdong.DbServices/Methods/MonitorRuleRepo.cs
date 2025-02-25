﻿using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Monitor;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class MonitorRuleRepo : ExaminationRepository<MonitorRule>, IMonitorRuleRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public MonitorRuleRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<dynamic> GetRuleMini()
        {
            return await fsql.Get(conn_str).Select<MonitorRule>()
                .Where(u => u.IsDeleted == 0 && u.Status == MonitorRuleStatus.Enabled)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(u => new
                {
                    value = u.Id,
                    text = u.Title,
                    mix = u.StreamMix == 0 ? "不混流" : "混流",
                    count = u.StreamCount
                });
        }

        public async Task<bool> CreateMonitorRole(MainMonitorDto dto)
        {
            var monitorRule = new MonitorRule()
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                StreamMix = 0,
                StreamCount = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            if (dto.MonitorStreamConfigDtos.Count == 0)
            {
                return await addItemAsync(monitorRule) ==1 ;
            }
            using(var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var monitorStreamConfigRepo = uow.GetRepository<MonitorStreamConfig>();
                    var monitorRuleRepo = uow.GetRepository<MonitorRule>();
                    List<MonitorStreamConfig> monitorStreamConfigs = dto.MonitorStreamConfigDtos.Adapt<List<MonitorStreamConfig>>();
                    foreach (var item in monitorStreamConfigs)
                    {                        
                        item.MonitorRuleId = monitorRule.Id;                       
                    }
                    await monitorStreamConfigRepo.InsertAsync(monitorStreamConfigs);
                    await monitorRuleRepo.InsertAsync(monitorRule);

                    uow.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    uow.Rollback();
                    Logger.Error($"提交失败,{ex.Message},{ex.StackTrace}");
                    throw;
                }
            }
        }
    }
}
