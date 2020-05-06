using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace SingleSignOn.DB.Sql.Stores
{
    public interface IStoreSystemEmail
    {
        List<IdentitySystemEmail> GetAll(IdentitySystemEmail filter, int currentPage, int pageSize);
        int Insert(IdentitySystemEmail identity);
        bool Update(IdentitySystemEmail identity);
        IdentitySystemEmail GetById(int Id);
        IdentitySystemEmail GetEmailToResend(string receiverEmail, string action);
        bool Delete(int Id);
        List<IdentitySystemEmail> GetList(string keyword);
    }

    public class StoreSystemEmail : IStoreSystemEmail
    {
        private readonly string _connectionString;
        private RpsSystemEmail myRepository;

        public StoreSystemEmail() : this("SingleSignOnDB")
        {

        }

        public StoreSystemEmail(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsSystemEmail(_connectionString);
        }

        #region  Common

        public List<IdentitySystemEmail> GetAll(IdentitySystemEmail filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentitySystemEmail identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentitySystemEmail identity)
        {
            return myRepository.Update(identity);
        }

        public IdentitySystemEmail GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public IdentitySystemEmail GetEmailToResend(string receiverEmail, string action)
        {
            return myRepository.GetEmailToResend(receiverEmail, action);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }

        public List<IdentitySystemEmail> GetList(string keyword)
        {
            return myRepository.GetList(keyword);
        }

        #endregion
    }
}
