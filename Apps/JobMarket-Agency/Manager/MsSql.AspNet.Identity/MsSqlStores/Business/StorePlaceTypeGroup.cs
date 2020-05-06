using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Collections.Generic;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StorePlaceTypeGroup : IStorePlaceTypeGroup
    {
        private readonly string _connectionString;
        private RpsPlaceTypeGroup r;

        public StorePlaceTypeGroup()
            : this("PfoodDBConnection")
        {

        }

        public StorePlaceTypeGroup(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsPlaceTypeGroup(_connectionString);
        }

        public List<IdentityPlaceTypeGroup> GetAll(IdentityPlaceTypeGroup filter, int currentPage, int pageSize)
        {
            return r.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPlaceTypeGroup identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityPlaceTypeGroup identity)
        {
            return r.Update(identity);
        }

        public bool Delete(int id)
        {
            return r.Delete(id);
        }

        public IdentityPlaceTypeGroup GetById(int id)
        {
            return r.GetById(id);
        }

        public IdentityPlaceTypeGroup GetDetail(int id)
        {
            return r.GetDetail(id);
        }

        public IdentityPlaceTypeGroupLang GetLangDetail(int id)
        {
            return r.GetLangDetail(id);
        }

        public int AddNewLang(IdentityPlaceTypeGroupLang identity)
        {
            return r.AddNewLang(identity);
        }

        public int UpdateLang(IdentityPlaceTypeGroupLang identity)
        {
            return r.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return r.DeleteLang(id);
        }
    }

    public interface IStorePlaceTypeGroup
    {
        List<IdentityPlaceTypeGroup> GetAll(IdentityPlaceTypeGroup filter, int currentPage, int pageSize);

        int Insert(IdentityPlaceTypeGroup identity);
        bool Update(IdentityPlaceTypeGroup identity);
        bool Delete(int id);

        IdentityPlaceTypeGroup GetById(int id);
        IdentityPlaceTypeGroup GetDetail(int id);

        IdentityPlaceTypeGroupLang GetLangDetail(int id);
        int AddNewLang(IdentityPlaceTypeGroupLang identity);
        int UpdateLang(IdentityPlaceTypeGroupLang identity);
        bool DeleteLang(int id);
    }
}
