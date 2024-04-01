using Magic.Guangdong.DbServices.Interfaces;

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
    }
}
