using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class CertRepo :ExaminationRepository<Cert>,ICertRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public CertRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }
    }
}
