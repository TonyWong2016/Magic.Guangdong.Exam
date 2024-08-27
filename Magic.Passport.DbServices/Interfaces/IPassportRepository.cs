using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Magic.Passport.DbServices.Interfaces
{
    public interface IPassportRepository<T> where T:class
    {
        /// <summary>
        /// 取前x条
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        List<T> getList(int take = 10);

        /// <summary>
        /// 取符合条件的前x条
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        List<T> getList(Expression<Func<T, bool>> predicate, int take = 0);

        /// <summary>
        /// 分页检索，并返回总数
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<T> getList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, out long total);

        /// <summary>
        /// 取第一条
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> getOneAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 取前x条（异步）
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<List<T>> getListAsync(int take = 0);

        /// <summary>
        /// 取符合条件的前x条（异步）
        /// </summary>
        /// <param name="predicate">检索条件</param>
        /// <param name="take">取多少条，为0时则返回所有</param>
        /// <returns></returns>
        Task<List<T>> getListAsync(Expression<Func<T, bool>> predicate, int take = 0);

        /// <summary>
        /// 分页检索(异步检索不支持out，需另行获取条目总数)
        /// </summary>
        /// <param name="predicate">检索条件</param>
        /// <param name="pageIndex">起始页</param>
        /// <param name="pageSize">页容量</param>        
        /// <returns></returns>
        Task<List<T>> getListAsync(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize);

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        long getCount(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取记录数（异步）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<long> getCountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 是否存在符合条件的记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> getAnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 新增单条条目，返回影响条数
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int addItem(T t);
        /// <summary>
        /// 新增条目，返回自增主键id
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        long addItemsIdentity(T t);
        /// <summary>
        /// 新增多条条目，返回影响条数，效率较低！
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int addItems(List<T> list);

        /// <summary>
        /// 新增多条条目，不返回参数，效率高！
        /// </summary>
        /// <param name="list"></param>
        void addItemsBulk(List<T> list);

        /// <summary>
        /// 新增单条条目，返回影响条数（异步）
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<int> addItemAsync(T t);

        /// <summary>
        /// 新增多条条目，返回影响条数，效率较低！（异步）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<int> addItemsAsync(List<T> list);

        /// <summary>
        /// 新增多条条目，不返回参数，效率高！（异步）
        /// </summary>
        /// <param name="list"></param>
        Task addItemsBulkAsync(List<T> list);
        /// <summary>
        /// 新增条目，返回自增主键id（异步）
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<long> addItemsIdentityAsync(T t);

        /// <summary>
        /// 按条件删除条目
        /// dywhere 支持
        /// 主键值
        /// new[] { 主键值1, 主键值2 }
        /// 对象
        /// new[] { 对象1, 对象2 }
        /// new { id = 1 }
        /// var t1 = fsql.Delete<Topic>(new[] { 1, 2 }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1 OR `Id` = 2)
        /// var t2 = fsql.Delete<Topic>(new Topic { Id = 1, Title = "test" }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1)
        /// var t3 = fsql.Delete<Topic>(new[] { new Topic { Id = 1, Title = "test" }, new Topic { Id = 2, Title = "test" } }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1 OR `Id` = 2)
        /// var t4 = fsql.Delete<Topic>(new { id = 1 }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1)
        /// </summary>
        /// <param name="dywhere"></param>
        /// <returns></returns>
        int delItem(object dywhere);

        /// <summary>
        /// 按条件删除条目（异步）
        /// dywhere 支持
        /// 主键值
        /// new[] { 主键值1, 主键值2 }
        /// 对象
        /// new[] { 对象1, 对象2 }
        /// new { id = 1 }
        /// var t1 = fsql.Delete<Topic>(new[] { 1, 2 }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1 OR `Id` = 2)
        /// var t2 = fsql.Delete<Topic>(new Topic { Id = 1, Title = "test" }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1)
        /// var t3 = fsql.Delete<Topic>(new[] { new Topic { Id = 1, Title = "test" }, new Topic { Id = 2, Title = "test" } }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1 OR `Id` = 2)
        /// var t4 = fsql.Delete<Topic>(new { id = 1 }).ToSql();
        /// 等价于 DELETE FROM `Topic` WHERE (`Id` = 1)
        /// </summary>
        /// <param name="dywhere"></param>
        /// <returns></returns>
        Task<int> delItemAsync(object dywhere);

        /// <summary>
        /// 更新单条条目
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int updateItem(T t);

        /// <summary>
        /// 更新单条条目(异步)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<int> updateItemAsync(T t);
    }
}
