using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace BLL.DTO
{
    public class DetailuOrderDTO
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int Quantity { get; set; }
        public string Address { get; set; }

        // product
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCompany { get; set; }
        public double ProductPrice { get; set; }

        // user
        public int UserId { get; set; }
        public string UserLogin { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
