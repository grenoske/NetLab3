using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IwhOrderService
    {
        void DeliveryProductToWh(int? wOrderDtoId = null);
        void DeliveryProductToUser(int? uOrderDtoId = null);
        DetailwhOrderDTO GetDetailwhOrder(int? id);
        IEnumerable<whOrderDTO> SearchwhOrder(int? id);
        IEnumerable<whOrderDTO> GetOrders(string? filter = null, int page = 1, int amount = 5);
        IEnumerable<uOrderDTO> GetuOrders(string? filter = null, int page = 1, int amount = 5);
        void Dispose(); 
    }
}
