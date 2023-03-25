using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DAL.Entities;


namespace DAL.Interfaces
{
    public interface IProductQuantityRepository : IRepository<ProductQuantity>
    {
        void Update(ProductQuantity obj);
    }
}
