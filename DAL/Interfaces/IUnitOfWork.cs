using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; } 
        IuOrderRepository uOrders { get; }
        IwhOrderRepository whOrders { get; }
        IProductQuantityRepository ProductQuantity { get; }
        IUserRepository Users { get; }
        IWarehouseRepository Warehouse { get; }
        void Save();
    }
}
