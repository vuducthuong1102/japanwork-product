using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StoreGroupProperty : IStoreGroupProperty
    {
        private readonly string _connectionString;
        private RpsGroupProperty r;

        public StoreGroupProperty()
            : this("PfoodDBConnection")
        {

        }

        public StoreGroupProperty(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsGroupProperty(_connectionString);
        }

        public List<IdentityGroupProperty> GetByPage(IdentityGroupProperty filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityGroupProperty identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityGroupProperty identity)
        {
            return r.Update(identity);
        }

        public bool Delete(int id)
        {
            return r.Delete(id);
        }

        public IdentityGroupProperty GetById(int id)
        {
            return r.GetById(id);
        }

        public IdentityGroupProperty GetDetail(int id)
        {
            return r.GetDetail(id);
        }

        public List<IdentityGroupPropertyLang> GetLangDetail(int id)
        {
            return r.GetLangDetail(id);
        }

        public int AddNewLang(IdentityGroupPropertyLang identity)
        {
            return r.AddNewLang(identity);
        }

        public int UpdateLang(IdentityGroupPropertyLang identity)
        {
            return r.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return r.DeleteLang(id);
        }

        public List<IdentityGroupProperty> GetAll()
        {
            return r.GetAll();
        }
    }

    public interface IStoreGroupProperty
    {
        List<IdentityGroupProperty> GetByPage(IdentityGroupProperty filter, int currentPage, int pageSize);

        int Insert(IdentityGroupProperty identity);
        bool Update(IdentityGroupProperty identity);
        bool Delete(int id);

        IdentityGroupProperty GetById(int id);
        IdentityGroupProperty GetDetail(int id);

        List<IdentityGroupPropertyLang> GetLangDetail(int id);
        int AddNewLang(IdentityGroupPropertyLang identity);
        int UpdateLang(IdentityGroupPropertyLang identity);
        bool DeleteLang(int id);
        List<IdentityGroupProperty> GetAll();
    }
}
