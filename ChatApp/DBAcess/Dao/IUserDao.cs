using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.DBAcess.Dao
{

    public interface IUserDao
    {
        int Insert(string name);

        int Delete(int id);

        /// <summary>查询所有用户</summary>
        IEnumerable<User> GetAll();
    }
}
