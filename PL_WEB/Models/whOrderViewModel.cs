using BLL.DTO;
using System.ComponentModel.DataAnnotations;

namespace PL_WEB.Models
{
    public class whOrderViewModel
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Address { get; set; }

        public int ProductId { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }

        public static explicit operator whOrderViewModel(whOrderDTO whOrderDTO)
        {
            whOrderViewModel wOrderViewModel = new whOrderViewModel();
            wOrderViewModel.Id = whOrderDTO.Id;
            wOrderViewModel.Sum = whOrderDTO.Sum;
            wOrderViewModel.Quantity = whOrderDTO.Quantity; 
            wOrderViewModel.ProductId = whOrderDTO.ProductId;
            wOrderViewModel.Date = whOrderDTO.Date;
            wOrderViewModel.Status = whOrderDTO.Status;
            return wOrderViewModel;
        }
    }
}