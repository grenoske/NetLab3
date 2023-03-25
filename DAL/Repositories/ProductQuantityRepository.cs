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
    public class ProductQuantityRepository : Repository<ProductQuantity>, IProductQuantityRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductQuantityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ProductQuantity obj)
        {
            _db.ProductQuantity.Update(obj);
        }
    }
}
