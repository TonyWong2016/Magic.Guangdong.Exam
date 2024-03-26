using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Interfaces;
using FreeSql.Internal.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class ExaminationRepository<T> : IExaminationRepository<T> where T : class
    {
        IdleBus<IFreeSql> fsql = null; //单例注入idlebus
        const string conn_str = "db_exam";
        public ExaminationRepository(IdleBus<IFreeSql> fsql)
        {
            this.fsql = fsql;
        }

        public async Task<T> getOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await fsql.Get(conn_str).Select<T>().Where(predicate).FirstAsync();
        }
        public async Task<bool> getAnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await fsql.Get(conn_str).Select<T>().AnyAsync(predicate);
        }
        /// <summary>
        /// 取前x条
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<T> getList(int take)
        {
            return fsql.Get(conn_str).Select<T>().Take(take).ToList();
        }
        /// <summary>
        /// 取符合条件的前x条
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<T> getList(Expression<Func<T, bool>> predicate, int take)
        {
            if (take == 0)
                return fsql.Get(conn_str).Select<T>().Where(predicate).ToList();
            return fsql.Get(conn_str).Select<T>().Where(predicate).Take(take).ToList();
        }
        /// <summary>
        /// 分页检索，并返回总数
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<T> getList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, out long total)
        {
            return fsql.Get(conn_str).Select<T>().Where(predicate).Count(out total).Page(pageIndex, pageSize).ToList();
        }

        /// <summary>
        /// 分页检索（动态解析检索条件），并返回总数
        /// </summary>
        /// <param name="pageDto"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<T> getList(PageDto pageDto, out long total)
        {
            if (string.IsNullOrWhiteSpace(pageDto.whereJsonStr))
                return fsql.Get(conn_str).Select<T>().Count(out total).Page(pageDto.pageindex, pageDto.pagesize).ToList();

            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(pageDto.whereJsonStr);
            //string sql = fsql.Get(conn_str).Select<T>().WhereDynamicFilter(dyfilter).Count(out total).Page(pageDto.pageindex, pageDto.pagesize).ToSql();
            //Console.Write(sql);
            return fsql.Get(conn_str)
                .Select<T>()
                .WhereDynamicFilter(dyfilter)
                .OrderByPropertyName(pageDto.orderby,pageDto.isAsc)
                .Count(out total)
                .Page(pageDto.pageindex, pageDto.pagesize).ToList();
        }

        /// <summary>
        /// 取前x条（异步）
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<List<T>> getListAsync(int take)
        {
            return await fsql.Get(conn_str).Select<T>().Take(take).ToListAsync();
        }

        /// <summary>
        /// 取符合条件的前x条（异步）
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<List<T>> getListAsync(Expression<Func<T, bool>> predicate, int take)
        {
            if (take == 0)
                return await fsql.Get(conn_str).Select<T>().Where(predicate).ToListAsync();
            return await fsql.Get(conn_str).Select<T>().Where(predicate).Take(take).ToListAsync();
        }
        /// <summary>
        /// 分页检索(异步检索不支持out，需另行获取条目总数)
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>        
        /// <returns></returns>
        public async Task<List<T>> getListAsync(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize)
        {
            return await fsql.Get(conn_str).Select<T>().Where(predicate).Page(pageIndex, pageSize).ToListAsync();
        }

        /// <summary>
        /// 分页检索（动态解析检索条件）
        /// </summary>
        /// <param name="whereJsonStr"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<T>> getListAsync(PageDto pageDto)
        {
            if (string.IsNullOrWhiteSpace(pageDto.whereJsonStr))
                return await fsql.Get(conn_str).Select<T>().Page(pageDto.pageindex, pageDto.pagesize).ToListAsync();

            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(pageDto.whereJsonStr);
            return await fsql.Get(conn_str).Select<T>().WhereDynamicFilter(dyfilter).Page(pageDto.pageindex, pageDto.pagesize).ToListAsync();
        }

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public long getCount(Expression<Func<T, bool>> predicate)
        {
            return fsql.Get(conn_str).Select<T>().Where(predicate).Count();
        }
        /// <summary>
        /// 获取记录数（异步）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<long> getCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await fsql.Get(conn_str).Select<T>().Where(predicate).CountAsync();
        }

        /// <summary>
        /// 新增单条条目，返回影响条数
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int addItem(T t)
        {
            return fsql.Get(conn_str).Insert<T>(t).ExecuteAffrows();
        }

        /// <summary>
        /// 新增条目，返回自增主键id
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public long addItemsIdentity(T t)
        {
            return fsql.Get(conn_str).Insert(t).ExecuteIdentity();
        }

        /// <summary>
        /// 新增多条条目，返回影响条数，效率较低！
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int addItems(List<T> list)
        {
            return fsql.Get(conn_str).Insert(list).ExecuteAffrows();
        }
        /// <summary>
        /// 新增多条条目，不返回参数，效率高！
        /// </summary>
        /// <param name="list"></param>
        public void addItemsBulk(List<T> list)
        {
            fsql.Get(conn_str).Insert(list).ExecuteSqlBulkCopy();
        }
        /// <summary>
        /// 新增单条条目，返回影响条数（异步）
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<int> addItemAsync(T t)
        {
            return await fsql.Get(conn_str).Insert<T>(t).ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 新增条目，返回自增主键id（异步）
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<long> addItemsIdentityAsync(T t)
        {
            return await fsql.Get(conn_str).Insert(t).ExecuteIdentityAsync();
        }
        /// <summary>
        /// 新增多条条目，返回影响条数，效率较低！（异步）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<int> addItemsAsync(List<T> list)
        {
            return await fsql.Get(conn_str).Insert(list).ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 新增多条条目，不返回参数，效率高！（异步）
        /// </summary>
        /// <param name="list"></param>
        public async Task addItemsBulkAsync(List<T> list)
        {
            await fsql.Get(conn_str).Insert(list).ExecuteSqlBulkCopyAsync();
        }

        /// <summary>
        /// 按条件删除条目
        /// dywhere 支持
        /// 主键值
        /// new[] { 主键值1, 主键值2 }
        /// 对象
        /// new[] { 对象1, 对象2 }
        /// new { id = 1 }
        /// var t1 = fsql.Get(conn_str).Delete<Topic>(new[] { 1, 2 }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1 OR `Id` = 2)
        /// var t2 = fsql.Get(conn_str).Delete<Topic>(new Topic { Id = 1, Title = "test" }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1)
        /// var t3 = fsql.Get(conn_str).Delete<Topic>(new[] { new Topic { Id = 1, Title = "test" }, new Topic { Id = 2, Title = "test" } }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1 OR `Id` = 2)
        /// var t4 = fsql.Get(conn_str).Delete<Topic>(new { id = 1 }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1)
        /// </summary>
        /// <param name="dywhere"></param>
        /// <returns></returns>
        public int delItem(object dywhere)
        {
            return fsql.Get(conn_str).Delete<T>(dywhere).ExecuteAffrows();
        }

        /// <summary>
        /// 按条件删除条目（异步）
        /// dywhere 支持
        /// 主键值
        /// new[] { 主键值1, 主键值2 }
        /// 对象
        /// new[] { 对象1, 对象2 }
        /// new { id = 1 }
        /// var t1 = fsql.Get(conn_str).Delete<Topic>(new[] { 1, 2 }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1 OR `Id` = 2)
        /// var t2 = fsql.Get(conn_str).Delete<Topic>(new Topic { Id = 1, Title = "test" }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1)
        /// var t3 = fsql.Get(conn_str).Delete<Topic>(new[] { new Topic { Id = 1, Title = "test" }, new Topic { Id = 2, Title = "test" } }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1 OR `Id` = 2)
        /// var t4 = fsql.Get(conn_str).Delete<Topic>(new { id = 1 }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1)
        /// </summary>
        /// <param name="dywhere"></param>
        /// <returns></returns>
        public async Task<int> delItemAsync(object dywhere)
        {
            return await fsql.Get(conn_str).Delete<T>(dywhere).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 更新单条条目
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int updateItem(T t)
        {
            return fsql.Get(conn_str).Update<T>().SetSource(t).ExecuteAffrows();
        }

        /// <summary>
        /// 更新单条条目(异步)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<int> updateItemAsync(T t)
        {
            return await fsql.Get(conn_str).Update<T>().SetSource(t).ExecuteAffrowsAsync();
        }

        public async Task<T> insertOrUpdateAsync(T t)
        {
            return await fsql.Get(conn_str).GetRepository<T>().InsertOrUpdateAsync(t);
        }

        /// <summary>
        /// dywhere 支持
        /// 主键值
        /// new[] { 主键值1, 主键值2 }
        /// 对象
        /// new[] { 对象1, 对象2 }
        /// new { id = 1 }
        /// 例1
        /// fsql.Get(conn_str).Update<Topic>(1)
        /// .Set(a => a.CreateTime, DateTime.Now)
        /// .ExecuteAffrows();
        /// 等价于
        /// UPDATE `Topic` SET `CreateTime` = '2018-12-08 00:04:59' 
        /// WHERE (`Id` = 1)
        /// 更多例子，参照：http://freesql.net/guide/update.html
        /// </summary>
        /// <param name="dywhere"></param>
        /// <returns></returns>
        /// 其他更新方法不适合集成到仓储类，可按需实现
        ///public int updateItem(object dywhere)
        //{
        //    return fsql.Get(conn_str).Update<T>(dywhere).ExecuteAffrows();
        //}
    }
}
