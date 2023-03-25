using BLL.DTO;
using System.ComponentModel.DataAnnotations;

namespace PL_WEB.Models
{
    public class DetailwhOrderViewModel
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCompany { get; set; }
        public double ProductPrice { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }

        public static explicit operator DetailwhOrderViewModel(DetailwhOrderDTO whOrderDTO)
        {
            DetailwhOrderViewModel wOrderViewModel = new DetailwhOrderViewModel();
            wOrderViewModel.Id = whOrderDTO.Id;
            wOrderViewModel.Sum = whOrderDTO.Sum;
            wOrderViewModel.Quantity = whOrderDTO.Quantity; 
            wOrderViewModel.ProductId = whOrderDTO.ProductId;
            wOrderViewModel.ProductName = whOrderDTO.ProductName;
            wOrderViewModel.ProductCompany = whOrderDTO.ProductCompany;
            wOrderViewModel.ProductPrice = whOrderDTO.ProductPrice; 
            wOrderViewModel.Date = whOrderDTO.Date;
            wOrderViewModel.Status = whOrderDTO.Status;
            return wOrderViewModel;
        }
    }
}