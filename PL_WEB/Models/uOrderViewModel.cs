using BLL.DTO;
using System.ComponentModel.DataAnnotations;

namespace PL_WEB.Models
{
    public class uOrderViewModel
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Address { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }

        public static explicit operator uOrderViewModel(uOrderDTO uOrderDTO)
        {
            uOrderViewModel uOrderViewModel = new uOrderViewModel();
            uOrderViewModel.Id = uOrderDTO.Id;
            uOrderViewModel.Sum = uOrderDTO.Sum;
            uOrderViewModel.Quantity = uOrderDTO.Quantity;
            uOrderViewModel.Address = uOrderDTO.Address;    
            uOrderViewModel.ProductId = uOrderDTO.ProductId;
            uOrderViewModel.UserId = uOrderDTO.UserId;
            uOrderViewModel.Date = uOrderDTO.Date;
            uOrderViewModel.Status = uOrderDTO.Status;
            return uOrderViewModel;
        }
    }
}