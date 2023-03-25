using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace BLL.DTO
{
    public class uOrderDTO
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int Quantity { get; set; }
        public string Address { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
