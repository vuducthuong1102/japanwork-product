using System.Configuration;
using SingleSignOn.DB.Sql.Repositories;
using SingleSignOn.DB.Sql.Entities;
using System.Collections.Generic;

namespace SingleSignOn.DB.Sql.Stores
{
    public class StoreDocumentApi : IStoreDocumentApi
    {
        private readonly string _connectionString;
        private RpsDocumentApi myRepository;

        public StoreDocumentApi()
            : this("SingleSignOnDB")
        {

        }

        public StoreDocumentApi(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsDocumentApi(_connectionString);
        }

        public void Insert(string linkUrl, string Data)
        {
            myRepository.InsertDocumentApi(linkUrl, Data);
        }
        public IdentityDocumentApi GetByLinkUrl(string linkUrl)
        {
            return myRepository.GetByLinkUrl(linkUrl);
        }
    }

    public interface IStoreDocumentApi
    {
        void Insert(string linkUrl, string Data);

        IdentityDocumentApi GetByLinkUrl(string linkUrl);
    }
}
