using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class uOrder
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int Quantity { get; set; }
        public string Address { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int whOrderId { get; set; }
        public whOrder whOrder { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
