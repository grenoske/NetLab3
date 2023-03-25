using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.EF;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public class whOrderRepository : Repository<whOrder>, IwhOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public whOrderRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(whOrder obj)
        {
            _db.whOrders.Update(obj);
        }
    }
}
