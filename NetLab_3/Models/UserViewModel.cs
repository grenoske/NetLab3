using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace PL.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public UserViewModel(UserDTO modelFomBLL)
        {
            Id = modelFomBLL.Id;
            Login = modelFomBLL.Login;
            Password = modelFomBLL.Password;
        }

        public override string ToString()
        {
            return "Product: id:" + Id + " login:" + Login;
        }
    }
}
