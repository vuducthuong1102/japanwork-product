using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreDevice
    {
        //List<IdentityJobSeekerDevice> GetAllDevicesByPage(dynamic filter, int currentPage, int pageSize);
        int JobSeekerUpdate(IdentityJobSeekerDevice identity);
        List<IdentityJobSeekerDevice> JobSeekerGetAllDevices(int job_seeker_id);
    }

    public class StoreDevice : IStoreDevice
    {
        private readonly string _connectionString;
        private RpsDevice myRepository;

        public StoreDevice() : this("JobMarketDB")
        {

        }

        public StoreDevice(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsDevice(_connectionString);
        }

        #region  Common

        public int JobSeekerUpdate(IdentityJobSeekerDevice identity)
        {
            return myRepository.JobSeekerUpdate(identity);
        }

        public List<IdentityJobSeekerDevice> JobSeekerGetAllDevices(int job_seeker_id)
        {
            return myRepository.JobSeekerGetAllDevices(job_seeker_id);
        }
        #endregion
    }
}
