using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreCs
    {
        List<IdentityCs> GetByPage(IdentityCs filter, int currentPage, int pageSize);
        List<IdentityCs> SearchByPage(dynamic filter, int currentPage, int pageSize);
        List<IdentityCs> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize);
        int Insert(IdentityCs identity);
        bool Update(IdentityCs identity);
        IdentityCs GetById(int id);
        IdentityCs GetDetail(int id);
        bool Delete(int id);
        //int SavePrintCode(IdentityCsPdfCode codeInfo);
        //int DeletePrintCode(IdentityCsPdfCode codeInfo);

        List<IdentityCs> GetList();
        List<IdentityCs> GetListByJobSeeker(IdentityCs filter, int currentPage, int pageSize);
        //List<IdentityCsPdfCode> GetListCodeByJobSeeker(int job_seeker_id);
        int Clone(IdentityCs identity);
        List<int> SetMainCs(IdentityCs info);

        List<IdentityCs> GetSuggestionsForInvitationByPage(dynamic filter, int currentPage, int pageSize);
    }

    public class StoreCs : IStoreCs
    {
        private readonly string _connectionString;
        private RpsCs myRepository;

        public StoreCs() : this("JobMarketDB")
        {

        }

        public StoreCs(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCs(_connectionString);
        }

        #region  Common

        public List<IdentityCs> GetByPage(IdentityCs filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityCs> SearchByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.SearchByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCs identity)
        {
            return myRepository.Insert(identity);
        }

        public int Clone(IdentityCs identity)
        {
            return myRepository.Clone(identity);
        }

        public bool Update(IdentityCs identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCs GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityCs GetDetail(int id)
        {
            return myRepository.GetDetail(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityCs> GetList()
        {
            return myRepository.GetList();
        }

        //public List<IdentityCs> GetListByJobSeeker(int job_id)
        //{
        //    return myRepository.GetListByJobSeeker(job_id);
        //}

        public List<IdentityCs> GetListByJobSeeker(IdentityCs filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJobSeeker(filter, currentPage, pageSize);
        }

        //public List<IdentityCsPdfCode> GetListCodeByJobSeeker(int job_seeker_id)
        //{
        //    return myRepository.GetListCodeByJobSeeker(job_seeker_id);
        //}

        //public int SavePrintCode(IdentityCsPdfCode codeInfo)
        //{
        //    return myRepository.SavePrintCode(codeInfo);
        //}

        //public int DeletePrintCode(IdentityCsPdfCode codeInfo)
        //{
        //    return myRepository.DeletePrintCode(codeInfo);
        //}

        public List<int> SetMainCs(IdentityCs info)
        {
            return myRepository.SetMainCs(info);
        }

        public List<IdentityCs> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetSuggestionsByPage(filter, currentPage, pageSize);
        }

        public List<IdentityCs> GetSuggestionsForInvitationByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetSuggestionsForInvitationByPage(filter, currentPage, pageSize);
        }
        #endregion
    }
}
