using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Entities
{
    public class whOrder
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int uOrderId { get; set; }
        public uOrder uOrder { get; set; }

        public int Quantity { get; set; }

        public double Sum { get; set; }
        public string Status { get; set; }

        public DateTime Date { get; set; }
    }
}
