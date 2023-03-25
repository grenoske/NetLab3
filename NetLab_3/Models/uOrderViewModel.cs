﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace PL.Models
{
    public class uOrderViewModel
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int Quantity { get; set; }
        public string Address { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }

        public uOrderViewModel(uOrderDTO modelFomBLL)
        {
            Id = modelFomBLL.Id;
            Sum = modelFomBLL.Sum;
            Quantity = modelFomBLL.Quantity;    
            Address = modelFomBLL.Address;
            ProductId = modelFomBLL.ProductId;
            UserId = modelFomBLL.UserId;
            Date = modelFomBLL.Date;    
            Status = modelFomBLL.Status;
        }

        public override string ToString()
        {
            return "Order: id: "+ Id + " Status:" + Status + " ProductId:" + ProductId + " Quantity:" + Quantity + " Address:" + Address + " UserId:" + UserId + " Date:" + Date;
        }
    }
}
