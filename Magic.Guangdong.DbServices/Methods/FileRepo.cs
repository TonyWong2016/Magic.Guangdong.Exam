using Magic.Guangdong.DbServices.Dtos.Material;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using System.Security.AccessControl;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class FileRepo : ExaminationRepository<Entities.File>, IFileRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public FileRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<List<MaterialResponseDto>> GetMaterials(string connId, string connName)
        {
            if (!string.IsNullOrEmpty(connName) && connName.ToLower() == "question")
            {
                var questionItem = await fsql.Get(conn_str).Select<QuestionItem>()
                    .Where(u => u.QuestionId == Convert.ToInt64(connId))
                    .Where(u=>u.IsDeleted==0)
                    .ToListAsync(u => new
                    {
                        itemId = u.Id.ToString()
                    });
                var itemIds = questionItem.Select(u => u.itemId).ToArray();

                return (await fsql.Get(conn_str).Select<Entities.File>()
                    .Where(u =>
                        (u.ConnId == connId && u.ConnName == connName) ||
                        (u.ConnName == "QuestionItem" && itemIds.Contains(u.ConnId))
                    )
                    .Where(u => u.IsDeleted == 0)
                    .ToListAsync())
                    .Adapt<List<MaterialResponseDto>>();

            }
            return (await fsql.Get(conn_str).Select<Entities.File>()
                .WhereIf(!string.IsNullOrEmpty(connId), u => u.ConnId == connId)
                .WhereIf(!string.IsNullOrEmpty(connName), u => u.ConnName == connName)
                .Where(u => u.IsDeleted == 0)
                .ToListAsync())
                .Adapt<List<MaterialResponseDto>>();
        }
    }
}
