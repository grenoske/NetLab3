using BLL.DTO;
using System.ComponentModel.DataAnnotations;

namespace PL_WEB.Models
{
    public class DetailuOrderViewModel
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int Quantity { get; set; }
        public string Address { get; set; }

        // product
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCompany { get; set; }
        public double ProductPrice { get; set; }

        // user
        public int UserId { get; set; }
        public string UserLogin { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }

        public static explicit operator DetailuOrderViewModel(DetailuOrderDTO uOrderDTO)
        {
            DetailuOrderViewModel uOrderViewModel = new DetailuOrderViewModel();
            uOrderViewModel.Id = uOrderDTO.Id;
            uOrderViewModel.Sum = uOrderDTO.Sum;
            uOrderViewModel.Quantity = uOrderDTO.Quantity;
            uOrderViewModel.Address = uOrderDTO.Address;    
            uOrderViewModel.ProductId = uOrderDTO.ProductId;
            uOrderViewModel.ProductName = uOrderDTO.ProductName;
            uOrderViewModel.ProductCompany = uOrderDTO.ProductCompany;
            uOrderViewModel.ProductPrice = uOrderDTO.ProductPrice;
            uOrderViewModel.UserId = uOrderDTO.UserId;
            uOrderViewModel.UserLogin = uOrderDTO.UserLogin;
            uOrderViewModel.Date = uOrderDTO.Date;
            uOrderViewModel.Status = uOrderDTO.Status;
            return uOrderViewModel;
        }
    }
}