using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace PL.Models
{
    public class whOrderViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public double Sum { get; set; }
        public string Status { get; set; }

        public DateTime Date { get; set; }

        public whOrderViewModel(whOrderDTO modelFomBLL)
        {
            Id = modelFomBLL.Id;
            ProductId = modelFomBLL.ProductId;
            Quantity = modelFomBLL.Quantity;
            Sum = modelFomBLL.Sum;
            Date = modelFomBLL.Date;    
            Status = modelFomBLL.Status;
        }

        public override string ToString()
        {
            return "Order: id:"+ Id + " Status:" + Status + " Quantity:" + Quantity + " Sum:" + Sum + " ProductId:" + ProductId + " Date:" + Date;
        }
    }
}
