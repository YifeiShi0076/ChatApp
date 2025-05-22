using ChatApp.DBAcess.Dao;
using ChatApp.DBAcess.Impl;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Service
{
    public class UserService
    {
        private readonly IUserDao _dao = new IUserDaoImpl();
        public bool AddUser(string name)
            => _dao.Insert(name) > 0;

        public bool RemoveUser(int id)
            => _dao.Delete(id) > 0;

        public IEnumerable<User> GetAllUsers()
            => _dao.GetAll();
    }
}
