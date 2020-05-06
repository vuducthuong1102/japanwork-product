using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StorePriceType : IStorePriceType
    {
        private readonly string _connectionString;
        private RpsPriceType r;

        public StorePriceType()
            : this("PfoodDBConnection")
        {

        }

        public StorePriceType(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsPriceType(_connectionString);
        }

        public List<IdentityPriceType> GetAll(IdentityPriceType filter, int currentPage, int pageSize)
        {
            return r.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPriceType identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityPriceType identity)
        {
            return r.Update(identity);
        }

        public bool Delete(int id)
        {
            return r.Delete(id);
        }

        public IdentityPriceType GetById(int id)
        {
            return r.GetById(id);
        }

        public IdentityPriceType GetDetail(int id)
        {
            return r.GetDetail(id);
        }

        public IdentityPriceTypeLang GetLangDetail(int id)
        {
            return r.GetLangDetail(id);
        }

        public int AddNewLang(IdentityPriceTypeLang identity)
        {
            return r.AddNewLang(identity);
        }

        public int UpdateLang(IdentityPriceTypeLang identity)
        {
            return r.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return r.DeleteLang(id);
        }
    }

    public interface IStorePriceType
    {
        List<IdentityPriceType> GetAll(IdentityPriceType filter, int currentPage, int pageSize);

        int Insert(IdentityPriceType identity);
        bool Update(IdentityPriceType identity);
        bool Delete(int id);

        IdentityPriceType GetById(int id);
        IdentityPriceType GetDetail(int id);

        IdentityPriceTypeLang GetLangDetail(int id);
        int AddNewLang(IdentityPriceTypeLang identity);
        int UpdateLang(IdentityPriceTypeLang identity);
        bool DeleteLang(int id);
    }
}
