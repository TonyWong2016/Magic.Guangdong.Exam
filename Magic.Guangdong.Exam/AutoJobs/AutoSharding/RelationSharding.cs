using Coravel.Invocable;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace Magic.Guangdong.Exam.AutoJobs.AutoSharding
{
    public class RelationSharding : IInvocable
    {
        private readonly IRelationRepo _relationRepo;
        public RelationSharding(IRelationRepo relationRepo) { 
            _relationRepo = relationRepo;
        }

        public async Task Invoke()
        {
            Assistant.Logger.Warning("Relation表开始执行自动分表操作");
            try
            {
                //await _relationRepo.AutoShardingRelation()
                if (DateTime.Now.Month == 1 && DateTime.Now.Day == 1 && await _relationRepo.AutoShardingRelation(Guid.Empty))
                {

                    await Assistant.EmailKitHelper.SendEMailToManagerMsgAsync("考试系统数据库（magic.guangdong.exam）分表成功，magic.guangdong.exam库的Relation表，分表完成，检测无误后请手动清空Relation表中IsDeleted标记为1的记录，以加速当年的业务调用效率");
                }
            }
            catch (Exception ex) {
                await Assistant.EmailKitHelper.SendEMailToDevMsgAsync("考试系统数据库（magic.guangdong.exam）分表失败，请尽快查看" + ex.Message);
                Assistant.Logger.Error(ex);
            }
        }
    }
}
