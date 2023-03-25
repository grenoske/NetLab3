using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IuOrderService
    {
        void MakeOrder(uOrderDTO orderDto);
        ProductDTO GetProduct(int? id);
        DetailProductDTO GetDetailProduct(int? id);
        DetailuOrderDTO GetDetailuOrder(int? id);
        void CanceluOrder(int id);
        void AddProduct(ProductDTO productDTO);
        void RemoveProduct(int id);
        IEnumerable<ProductDTO> SearchProduct(string Name);
        IEnumerable<uOrderDTO> SearchuOrder(int? id);
        IEnumerable<uOrderDTO> SearchUserOrder(int? id, int? UserId);
        IEnumerable<ProductDTO> GetProducts(int page = 1, int amount = 5);
        IEnumerable<uOrderDTO> GetOrders();
        IEnumerable<uOrderDTO> GetUserOrders(int? Useridint, int page = 1, int amount = 5);
        void Dispose(); 
    }
}
