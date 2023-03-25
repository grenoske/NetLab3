using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.EF;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Products = new ProductRepository(_db);
            uOrders = new uOrderRepository(_db);
            whOrders = new whOrderRepository(_db);
            Warehouse = new WarehouseRepository(_db);
            ProductQuantity = new ProductQuantityRepository(_db);
            Users = new UserRepository(_db);
            
        }

        public IProductRepository Products { get; private set; }  
        public IuOrderRepository uOrders { get; private set; }
        public IUserRepository Users { get; private set; }
        public IwhOrderRepository whOrders { get; private set; }
        public IWarehouseRepository Warehouse { get; private set; }
        public IProductQuantityRepository ProductQuantity { get; private set; }


        public void Save()
        {
            _db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

