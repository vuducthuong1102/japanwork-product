using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreCv
    {
        List<IdentityCv> GetByPage(IdentityCv filter, int currentPage, int pageSize);
        List<IdentityCv> SearchByPage(dynamic filter, int currentPage, int pageSize);
        List<IdentityCv> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize);
        int Insert(IdentityCv identity);
        bool Update(IdentityCv identity);
        IdentityCv GetById(int id);
        IdentityCv GetDetail(int id);
        bool Delete(int id);
        int SavePrintCode(IdentityCvPdfCode codeInfo);
        int DeletePrintCode(IdentityCvPdfCode codeInfo);

        List<IdentityCv> GetList();
        List<IdentityCv> GetListByJobSeeker(IdentityCv filter, int currentPage, int pageSize);

        List<IdentityCv> GetListCVSentToAgency(IdentityCv filter, int currentPage, int pageSize);
        List<IdentityCvPdfCode> GetListCodeByJobSeeker(int job_seeker_id);
        int Clone(IdentityCv identity);
        List<int> SetMainCv(IdentityCv info);

        List<IdentityCv> GetSuggestionsForInvitationByPage(dynamic filter, int currentPage, int pageSize);
        List<IdentityCv> GetByAgency(IdentityCv filter, int currentPage, int pageSize);

        #region Admin

        List<IdentityCv> M_GetListByJobSeeker(dynamic filter);

        #endregion
    }

    public class StoreCv : IStoreCv
    {
        private readonly string _connectionString;
        private RpsCv myRepository;

        public StoreCv() : this("JobMarketDB")
        {

        }

        public StoreCv(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCv(_connectionString);
        }

        #region  Common

        public List<IdentityCv> GetByPage(IdentityCv filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityCv> SearchByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.SearchByPage(filter, currentPage, pageSize);
        }
        public List<IdentityCv> GetByAgency(IdentityCv filter, int currentPage, int pageSize)
        {
            return myRepository.GetByAgency(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCv identity)
        {
            return myRepository.Insert(identity);
        }

        public int Clone(IdentityCv identity)
        {
            return myRepository.Clone(identity);
        }

        public bool Update(IdentityCv identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCv GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityCv GetDetail(int id)
        {
            return myRepository.GetDetail(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityCv> GetList()
        {
            return myRepository.GetList();
        }

        //public List<IdentityCv> GetListByJobSeeker(int job_id)
        //{
        //    return myRepository.GetListByJobSeeker(job_id);
        //}

        public List<IdentityCv> GetListByJobSeeker(IdentityCv filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJobSeeker(filter, currentPage, pageSize);
        }
        public List<IdentityCv> GetListCVSentToAgency(IdentityCv filter, int currentPage, int pageSize)
        {
            return myRepository.GetListCVSentToAgency(filter, currentPage, pageSize);
        }
        public List<IdentityCvPdfCode> GetListCodeByJobSeeker(int job_seeker_id)
        {
            return myRepository.GetListCodeByJobSeeker(job_seeker_id);
        }

        public int SavePrintCode(IdentityCvPdfCode codeInfo)
        {
            return myRepository.SavePrintCode(codeInfo);
        }

        public int DeletePrintCode(IdentityCvPdfCode codeInfo)
        {
            return myRepository.DeletePrintCode(codeInfo);
        }

        public List<int> SetMainCv(IdentityCv info)
        {
            return myRepository.SetMainCv(info);
        }

        public List<IdentityCv> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetSuggestionsByPage(filter, currentPage, pageSize);
        }

        public List<IdentityCv> GetSuggestionsForInvitationByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetSuggestionsForInvitationByPage(filter, currentPage, pageSize);
        }
        #endregion

        #region Admin

        public List<IdentityCv> M_GetListByJobSeeker(dynamic filter)
        {
            return myRepository.M_GetListByJobSeeker(filter);
        }

        #endregion
    }
}
