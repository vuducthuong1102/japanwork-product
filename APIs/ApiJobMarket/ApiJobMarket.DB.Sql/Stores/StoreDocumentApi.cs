using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Stores
{
    public class StoreDocumentApi : IStoreDocumentApi
    {
        private readonly string _connectionString;
        private RpsDocumentApi myRepository;

        public StoreDocumentApi()
            : this("JobMarketDB")
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
