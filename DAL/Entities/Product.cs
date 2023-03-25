using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public double Price { get; set; }
        public ProductQuantity ProductQuantity{ get; set; }
        public ICollection<uOrder> Orders { get; set; }
        public ICollection<whOrder> whOrders { get; set; }
    }
}
