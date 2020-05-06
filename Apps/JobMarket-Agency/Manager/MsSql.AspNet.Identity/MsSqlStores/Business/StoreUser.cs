using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;

namespace MsSql.AspNet.Identity.Stores
{
    public class StoreUser : IStoreUser
    {
        private readonly string _connectionString;
        private RpsUser myRepository;

        public StoreUser()
            : this("DefaultConnection")
        {

        }

        public StoreUser(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsUser(_connectionString);
        }

        public IdentityUser GetUserById(int userId)
        {
            return myRepository.GetDetailById(userId);
        }

        public List<IdentityUser> GetByPage(dynamic filter)
        {
            return myRepository.GetByPage(filter);
        }
    }

    public interface IStoreUser
    {
        IdentityUser GetUserById(int userId);
        List<IdentityUser> GetByPage(dynamic filter);        
    }
}
