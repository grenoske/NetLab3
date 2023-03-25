using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DAL.Entities;


namespace DAL.Interfaces
{
    public interface IwhOrderRepository : IRepository<whOrder>
    {
        void Update(whOrder obj);
    }
}
