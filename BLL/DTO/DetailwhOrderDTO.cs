using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class DetailwhOrderDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCompany { get; set; }
        public double ProductPrice { get; set; }

        public int Quantity { get; set; }

        public double Sum { get; set; }
        public string Status { get; set; }

        public DateTime Date { get; set; }
    }
}
