using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StoreMember : IStoreMember
    {
        private readonly string _connectionString;
        private RpsMember r;

        public StoreMember()
            : this("SingleSignOnDB")
        {

        }

        public StoreMember(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsMember(_connectionString);
        }

        public List<IdentityMember> GetByPage(IdentityMember filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityMember identity, out int code)
        {
            return r.Insert(identity, out code);
        }

        public bool Update(IdentityMember identity, out int code)
        {
            return r.Update(identity, out code);
        }

        public bool Delete(int id)
        {
            return r.Delete(id);
        }

        public IdentityMember GetById(int id)
        {
            return r.GetById(id);
        }

        public List<IdentityMember> GetList(string keyword)
        {
            return r.GetList(keyword);
        }
    }

    public interface IStoreMember
    {
        List<IdentityMember> GetByPage(IdentityMember filter, int currentPage, int pageSize);

        int Insert(IdentityMember identity, out int code);
        bool Update(IdentityMember identity, out int code);
        bool Delete(int id);

        IdentityMember GetById(int id);
        List<IdentityMember> GetList(string keyword);
    }
}
