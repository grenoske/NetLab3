using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        void RegUser(UserDTO userDto);
        UserDTO GetUser(string Login, string Password);
        IEnumerable<UserDTO> GetUsers();
        void Dispose(); 
    }
}
