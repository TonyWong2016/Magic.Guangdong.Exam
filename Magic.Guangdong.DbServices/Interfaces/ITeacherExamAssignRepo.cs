using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface ITeacherExamAssignRepo : IExaminationRepository<TeacherExamAssign>
    {
        List<TeacherExamAssignView> GetTeacherExamAssigns(PageDto pageDto, out long total);
        /// <summary>
        /// 0
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> AssginTeacher(TeacherExamAssignSingleTeacherDto dto);

        /// <summary>
        /// 给一场考试分配多个老师
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> AssginExam(TeacherExamAssignSingleExamDto dto);

        /// <summary>
        /// 给一个老师分配一场考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> AssignSingle(TeacherExamAssignSingleDto dto);

        /// <summary>
        /// 移除指定老师分配的所有考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> RemoveAssignTeacher(TeacherExamAssignSingleTeacherDto dto);

        /// <summary>
        /// 移除指定考试分配的所有老师
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> RemoveExam(TeacherExamAssignSingleExamDto dto);

        /// <summary>
        /// 移除指定老师分配的一场指定考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> RemoveAssignSingle(TeacherExamAssignSingleDto dto);

        /// <summary>
        /// 分配多个指定老师分配的多场指定考试
        /// 表单提交时，数组类型可能识别不到，前置转换一下
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> AssignByString(TeacherExamAssignStringDto dto);

        /// <summary>
        /// 分配多个指定老师分配的多场指定考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> Assign(TeacherExamAssignDto dto);

        /// <summary>
        /// 移除多个指定的老师分配的多场指定的考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> RemoveAssign(TeacherExamAssignDto dto);
    }
}
