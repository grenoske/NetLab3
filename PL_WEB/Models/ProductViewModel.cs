using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace PL_WEB.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public double Price { get; set; }

        public static explicit operator ProductViewModel(ProductDTO productDTO)
        {
            ProductViewModel PVM = new ProductViewModel();
            PVM.Id = productDTO.Id;
            PVM.Name = productDTO.Name;
            PVM.Company = productDTO.Company;
            PVM.Price = productDTO.Price;
            return PVM;
        }
    }
}
