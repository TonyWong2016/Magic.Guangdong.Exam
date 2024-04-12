using Magic.Guangdong.DbServices.Dtos.Order;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IOrderRepo:IExaminationRepository<Order>
    {
        Task<bool> SyncOrderInfo(SyncOrderDto dto);
    }
}
