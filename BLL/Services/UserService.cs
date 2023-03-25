using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.Entities;
using DAL.Interfaces;
using BLL.Infrastructure;
using BLL.Interfaces;
using AutoMapper;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        enum userTypes { customer, worker, admin }
        Dictionary<userTypes, string> dicStats = new Dictionary<userTypes, string>
        {
            { userTypes.customer, "customer" },         // 
            { userTypes.worker, "worker" },             // 
            { userTypes.admin, "admin"}                 // 

        };
        IUnitOfWork Database { get; set; }
        public UserService(IUnitOfWork unitOfWork)
        {
            Database = unitOfWork;
        }
        public UserDTO GetUser(string login, string password)
        {
            if (login == null)
                throw new ValidationException("Login is Null", "");
            var user = Database.Users.GetFirstOrDefault(u => u.Login == login);
            if (user == null)
                throw new ValidationException("User not Found", "");
            if (password != user.Password)
                throw new ValidationException("Password is Incorrect", "");

            return new UserDTO { Id = user.Id, Login = user.Login, Password = user.Password, Role = user.Role };
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<User>, List<UserDTO>>(Database.Users.GetAll());
        }

        public void RegUser(UserDTO userDto)
        {
            User user = Database.Users.GetFirstOrDefault(u => u.Login == userDto.Login);

            if (user != null)
                throw new ValidationException("User is already exists", "");

            var enumDisplayStatus = (userTypes)userTypes.customer;
            string stringValue = enumDisplayStatus.ToString();
            User usera = new User()
            {
                Date = DateTime.Now,
                Login = userDto.Login,
                Password = userDto.Password,
                Role = stringValue,
            };
            Database.Users.Add(usera);
            Database.Save();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
