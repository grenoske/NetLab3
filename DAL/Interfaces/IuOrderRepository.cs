using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DAL.Entities;


namespace DAL.Interfaces
{
    public interface IuOrderRepository : IRepository<uOrder>
    {
        void Update(uOrder obj);
    }
}
