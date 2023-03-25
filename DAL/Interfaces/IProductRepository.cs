using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DAL.Entities;


namespace DAL.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);
    }
}
