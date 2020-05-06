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
    public interface IStoreInterviewProcess
    {
        List<IdentityInterviewProcess> GetListByCandidate_Id(int candidate_id);
        List<IdentityInterviewProcess> GetListByCandidate_Ids(string candidate_ids);
        int Insert(IdentityInterviewProcess identity);
        bool Update(IdentityInterviewProcess identity);
        IdentityInterviewProcess GetById(int id);
        bool Delete(int id);
    }
    public class StoreInterviewProcess : IStoreInterviewProcess
    {
        private readonly string _connectionString;
        private RpsInterviewProcess myRepository;

        public StoreInterviewProcess() : this("JobMarketDB")
        {

        }

        public StoreInterviewProcess(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsInterviewProcess(_connectionString);
        }

        #region  Common

        public List<IdentityInterviewProcess> GetListByCandidate_Id(int candidate_id)
        {
            return myRepository.GetListByCandidate_Id(candidate_id);
        }

        public List<IdentityInterviewProcess> GetListByCandidate_Ids(string candidate_ids)
        {
            return myRepository.GetListByCandidate_Ids(candidate_ids);
        }

        public int Insert(IdentityInterviewProcess identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityInterviewProcess identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityInterviewProcess GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }
        #endregion
    }
}
