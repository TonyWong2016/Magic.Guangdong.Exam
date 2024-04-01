using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Dtos.System.Admins;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IAdminRepo : IExaminationRepository<Admin>
    {
        List<AdminListDto> GetAdminList(AdminListPageDto dto, out long total);

        Task<bool> CreateAdmin(AdminDto dto);

        Task<bool> UpdateAdmin(AdminDto dto);
    }
}
