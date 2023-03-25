using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace PL_WEB.Models
{
    public class DetailProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
    

        public static explicit operator DetailProductViewModel(DetailProductDTO productDTO)
        {
            DetailProductViewModel PVM = new DetailProductViewModel();
            PVM.Id = productDTO.Id;
            PVM.Name = productDTO.Name;
            PVM.Company = productDTO.Company;
            PVM.Quantity = productDTO.Quantity;
            PVM.ReservedQuantity = productDTO.ReservedQuantity;
            PVM.Price = productDTO.Price;
            return PVM;
        }
    }
}
