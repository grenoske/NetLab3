using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace PL.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public double Price { get; set; }

        public ProductViewModel(ProductDTO modelFomBLL)
        {
            Id = modelFomBLL.Id;
            Name = modelFomBLL.Name;
            Company = modelFomBLL.Company;
            Price = modelFomBLL.Price;
        }

        public override string ToString()
        {
            return "Product: id:" + Id + " name:" + Name + " company:" + Company + " price:" + Price;
        }
    }
}
