using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using SixLabors.Fonts.Tables.AdvancedTypographic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class TagsRepo : ExaminationRepository<Tags>, ITagsRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public TagsRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<bool> CreateTagRelations(List<Dtos.System.Tags.TagDto> dtos)
        {
            var tagsRepo = fsql.Get(conn_str).GetRepository<Tags>();
            var titles = dtos.Select(u => u.Title).ToArray();
            if (await tagsRepo.Where(u => titles.Contains(u.Title)).AnyAsync())
            {
                return false;
            }           
            var hashRelations = dtos.Select(u=>u.HashRelation).ToArray();
            var tagRelationsRepo = fsql.Get(conn_str).GetRepository<TagRelations>();
            if (await tagRelationsRepo.Where(u => u.HashRelation.Equals(hashRelations)).AnyAsync())
            {
                return false;
            }
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {                    
                    tagsRepo.UnitOfWork = uow;
                    tagRelationsRepo.UnitOfWork = uow;
                    List<Tags> tagList = new List<Tags>();
                    List<TagRelations> tagRelationList = new List<TagRelations>();
                    foreach (var d in dtos)
                    {
                        var tag = new Tags() { Id=d.Id, Title = d.Title };

                        if (!string.IsNullOrEmpty(d.AssociationId) && !string.IsNullOrEmpty(d.TableName))
                        {
                            tagRelationList.Add(new TagRelations()
                            {
                                TagId = d.Id,
                                TableName = d.TableName,
                                AssociationId = d.AssociationId,
                                OriginalId = d.OriginalId,
                            });
                        }
                        tagList.Add(tag);
                    }

                    await tagsRepo.InsertAsync(tagList);
                    await tagRelationsRepo.InsertAsync(tagRelationList);
                    uow.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    Assistant.Logger.Error(ex);
                    return false;
                }
            }
            
            
        }
    
        

        public async Task<int> RemoveRelationsByTagId(long id,bool force=false)
        {
            var tagRepo = fsql.Get(conn_str).GetRepository<Tags>();
            if(!await tagRepo.Where(u=>u.Id == id).AnyAsync())
            {
                return -1;
            }
            var tag = await tagRepo.Where(u => u.Id == id).ToOneAsync();
            tag.IsDeleted = 1;
            tag.Title += "(已删除)";
            var tagRelationRepo = fsql.Get(conn_str).GetRepository<TagRelations>();
            var relations = await tagRelationRepo.Where(u => u.TagId == id).ToListAsync();
            if (relations.Count == 0) 
            {          
                tag.Title += "(已删除)";
                await tagRepo.UpdateAsync(tag);
                return 0;
            }
            var reportInfoRepo = fsql.Get(conn_str).GetRepository<ReportInfo>();
            if (await reportInfoRepo.Where(u => u.TagId == id).AnyAsync() && !force)
            {
                return 1;
            }

            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                tagRepo.UnitOfWork = uow;
                tagRelationRepo.UnitOfWork= uow;
                reportInfoRepo.UnitOfWork = uow;
                foreach (var rel in relations)
                {
                    rel.IsDeleted = 1;
                }
                var reports = await reportInfoRepo.Where(u => u.TagId == id).ToListAsync();
                if (reports.Count > 0)
                {
                    List<ReportInfo> reportInfos = new List<ReportInfo>(reports.Count);
                    foreach (var report in reports)
                    {
                        report.TagId = 0;
                        report.UpdatedAt = DateTime.Now;
                        reportInfos.Add(report);
                    }
                    await reportInfoRepo.UpdateAsync(reportInfos);
                }
                await tagRelationRepo.UpdateAsync(relations);
                await tagRepo.UpdateAsync(tag);
                uow.Commit();
                return 0;
            }
        }
    }
}
