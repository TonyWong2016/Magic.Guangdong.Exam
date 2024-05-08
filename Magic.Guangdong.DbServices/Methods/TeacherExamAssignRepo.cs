using FreeSql.Internal.Model;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Exam.UserAnswerRecord;
using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class TeacherExamAssignRepo : ExaminationRepository<TeacherExamAssign>, ITeacherExamAssignRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public TeacherExamAssignRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        


        /// <summary>
        /// 给一个老师分配多门考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> AssginTeacher(TeacherExamAssignSingleTeacherDto dto)
        {
            Guid[] teacherIds = new Guid[] { dto.teacherId };
            TeacherExamAssignDto teacherExamAssignDto = new TeacherExamAssignDto()
            {
                teacherIds = teacherIds,
                examIds = dto.examIds
            };
            return await Assign(teacherExamAssignDto);
        }

        /// <summary>
        /// 给一场考试分配多个老师
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> AssginExam(TeacherExamAssignSingleExamDto dto)
        {
            Guid[] examIds = new Guid[] { dto.examId };
            TeacherExamAssignDto teacherExamAssignDto = new TeacherExamAssignDto()
            {
                teacherIds = dto.teacherIds,
                examIds = examIds
            };
            return await Assign(teacherExamAssignDto);
        }

        /// <summary>
        /// 给一个老师分配一场考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> AssignSingle(TeacherExamAssignSingleDto dto)
        {
            Guid[] teacherIds = new Guid[] { dto.teacherId };
            Guid[] examIds = new Guid[] { dto.examId };
            TeacherExamAssignDto teacherExamAssignDto = new TeacherExamAssignDto()
            {
                teacherIds = teacherIds,
                examIds = examIds
            };
            return await Assign(teacherExamAssignDto);
        }

        /// <summary>
        /// 移除指定老师分配的所有考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> RemoveAssignTeacher(TeacherExamAssignSingleTeacherDto dto)
        {
            Guid[] teacherIds = new Guid[] { dto.teacherId };
            TeacherExamAssignDto teacherExamAssignDto = new TeacherExamAssignDto()
            {
                teacherIds = teacherIds,
                examIds = dto.examIds
            };
            return await RemoveAssign(teacherExamAssignDto);
        }

        /// <summary>
        /// 移除指定考试分配的所有老师
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> RemoveExam(TeacherExamAssignSingleExamDto dto)
        {
            Guid[] examIds = new Guid[] { dto.examId };
            TeacherExamAssignDto teacherExamAssignDto = new TeacherExamAssignDto()
            {
                teacherIds = dto.teacherIds,
                examIds = examIds
            };
            return await RemoveAssign(teacherExamAssignDto);
        }

        /// <summary>
        /// 移除指定老师分配的一场指定考试
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> RemoveAssignSingle(TeacherExamAssignSingleDto dto)
        {
            Guid[] teacherIds = new Guid[] { dto.teacherId };
            Guid[] examIds = new Guid[] { dto.examId };
            TeacherExamAssignDto teacherExamAssignDto = new TeacherExamAssignDto()
            {
                teacherIds = teacherIds,
                examIds = examIds
            };
            return await RemoveAssign(teacherExamAssignDto);
        }

        public async Task<int> AssignByString(TeacherExamAssignStringDto dto)
        {
            string[] teacherIdStrings = dto.teacherIds.Split(',');
            string[] examIdStrings = dto.examIds.Split(',');
            try
            {
                TeacherExamAssignDto teacherExamAssignDto = new TeacherExamAssignDto()
                {
                    teacherIds = teacherIdStrings.Select(value => new Guid(value)).ToArray(),
                    examIds = examIdStrings.Select(value => new Guid(value)).ToArray()
                };
                return await Assign(teacherExamAssignDto);
            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex.Message);
                return -1;
            }
        }

        public async Task<int> Assign(TeacherExamAssignDto dto)
        {
            if (!dto.teacherIds.Any() || !dto.examIds.Any())
            {
                return -1;
            }
            var teacherExamAssignRepo = fsql.Get(conn_str).GetRepository<TeacherExamAssign>();
            //Expression<Func<TeacherExamAssign, bool>> filter = p => dto.teacherIds.Contains(p.TeacherId) && p.IsDeleted==0;
            var filterExamIds = await teacherExamAssignRepo
                .Where(u => u.IsDeleted == 0)
                .Where(u => dto.teacherIds.Contains(u.TeacherId))
                .Where(u=> dto.examIds.Contains(u.ExamId))
                .ToListAsync(u => new {u.ExamId,u.TeacherId});
            List<TeacherExamAssign> list = new List<TeacherExamAssign>();
            foreach(Guid examId in dto.examIds)
            {                
                foreach(Guid teacherId in dto.teacherIds)
                {
                    if (filterExamIds.Any(u => u.ExamId == examId && u.TeacherId == teacherId))
                    {
                        continue;
                    }
                    list.Add(new TeacherExamAssign()
                    {
                        ExamId = examId,
                        TeacherId = teacherId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
            }
            if(list.Count == 0)
            {
                return 0;//这种说明没有新的分配
            }
            await teacherExamAssignRepo.InsertAsync(list);
            return list.Count;
        }

        public async Task<int> RemoveAssign(TeacherExamAssignDto dto)
        {
            if (!dto.teacherIds.Any() || !dto.examIds.Any())
            {
                return -1;
            }
            var teacherExamAssignRepo = fsql.Get(conn_str).GetRepository<TeacherExamAssign>();
            var list = await teacherExamAssignRepo
                .Where(u => u.IsDeleted == 0)
                .Where(u => dto.teacherIds.Contains(u.TeacherId))
                .Where(u => dto.examIds.Contains(u.ExamId))
                .ToListAsync();
            if (list.Count == 0)
            {
                return 0;//这种说明没有需要移除的分配关系
            }
            foreach(TeacherExamAssign teacherExamAssign in list)
            {
                teacherExamAssign.IsDeleted = 1;
                teacherExamAssign.UpdatedAt = DateTime.Now;
            }
            await teacherExamAssignRepo.UpdateAsync(list);
            return list.Count;
        }
    }
}
