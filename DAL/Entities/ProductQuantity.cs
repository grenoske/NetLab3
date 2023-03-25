using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class ProductQuantity
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        
        public Warehouse Warehouse { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
    }
}
