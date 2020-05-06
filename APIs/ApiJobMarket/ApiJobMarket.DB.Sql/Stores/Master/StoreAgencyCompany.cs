using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreAgencyCompany
    {
        List<IdentityAgencyCompany> GetCompaniesByAgencyId(int agency_id);

        List<IdentityAgencyCompany> GetAgencysByCompanyId(int company_id);

        bool InsertAgency(int company_id, List<int> ListAgencyId, int agency_parent_id);

        bool InsertCompany(int agency_id, List<int> ListCompanyId, int agency_parent_id);

        bool Delete(List<int> ListId);
    }

    public class StoreAgencyCompany : IStoreAgencyCompany
    {
        private readonly string _connectionString;
        private RpsAgencyCompany myRepository;

        public StoreAgencyCompany() : this("JobMarketDB")
        {

        }

        public StoreAgencyCompany(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsAgencyCompany(_connectionString);
        }

        #region  Common

        public List<IdentityAgencyCompany> GetCompaniesByAgencyId(int agency_id)
        {
            return myRepository.GetCompaniesByAgencyId(agency_id);
        }

        public List<IdentityAgencyCompany> GetAgencysByCompanyId(int company_id)
        {
            return myRepository.GetAgencysByCompanyId(company_id);
        }

        public bool InsertAgency(int company_id, List<int> ListAgencyId, int agency_parent_id)
        {
            return myRepository.InsertAgency(company_id, ListAgencyId, agency_parent_id);
        }
        public bool InsertCompany(int agency_id, List<int> ListCompanyId, int agency_parent_id)
        {
            return myRepository.InsertCompany(agency_id, ListCompanyId, agency_parent_id);
        }
        public bool Delete(List<int> ListId)
        {
            return myRepository.Delete(ListId);
        }
        #endregion
    }
}
