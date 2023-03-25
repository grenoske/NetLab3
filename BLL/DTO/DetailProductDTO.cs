using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace BLL.DTO
{
    public class DetailProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }  
    }
}
