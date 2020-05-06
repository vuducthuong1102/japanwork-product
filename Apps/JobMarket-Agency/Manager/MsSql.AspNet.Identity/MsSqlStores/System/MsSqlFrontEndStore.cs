using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity;

namespace MsSql.AspNet.Identity
{
    public class MsSqlFrontEndStore : IMsSqlFrontEndStore
    {
        private readonly string _connectionString;
        private MsSqlFrontEndRepository _r;

        public MsSqlFrontEndStore()
            : this("DefaultConnection")
        {

        }

        public MsSqlFrontEndStore(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _r = new MsSqlFrontEndRepository(_connectionString);                        
        }


        #region Settings

        public virtual Task<List<Setting>> LoadSettings(string settingType)
        {
            if (string.IsNullOrEmpty(settingType))
            {
                throw new ArgumentNullException("settingType");
            }

            var settings = _r.LoadSettings(settingType);
            return Task.FromResult<List<Setting>>(settings);
        }


        public virtual Task<bool> SaveSettings(List<Setting> settings)
        {
            if (settings == null || settings != null && settings.Count <= 0)
            {
                throw new ArgumentNullException("settings");
            }

            var rowsEffected = _r.SaveSettings(settings);

            return Task.FromResult<bool>(rowsEffected == settings.Count);
        }

        #endregion       
    }

    public interface IMsSqlFrontEndStore
    {      
        #region Settings

        Task<List<Setting>> LoadSettings(string settingType);

        Task<bool> SaveSettings(List<Setting> settings);

        #endregion       
    }
}
