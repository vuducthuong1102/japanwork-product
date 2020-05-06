using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreShop
    {
        List<IdentityShop> GetByPage(IdentityShop filter, int currentPage, int pageSize);
        int Insert(IdentityShop identity);
        bool Update(IdentityShop identity);
        IdentityShop GetById(int id);
        bool Delete(int id);
        List<IdentityShop> GetList(string keyword);
        List<IdentityShop> GetAssigned(int userId);
        bool UpdateMap(int shopId, string latitude, string longitude);
        int AssignToUser(int shopId, int userId, bool isOwner);
        bool DeleteAssignedUser(int shopId, int userId);
        List<IdentityShopMember> GetAssignedMembers(int shopId);
        int AssignCategory(int shopId, int categoryId);

        #region Meta data

        #region Images

        List<MetaShopImage> GetListImage(int Id);
        bool AddNewImage(MetaShopImage identity);
        bool RemoveImage(string Id);

        #endregion

        #region Amenities

        bool UpdateAmenities(int shopId, string selectedValues);

        #endregion

        #region NearPlaces

        string GetNearPlaces(int shopId);
        bool UpdateNearPlaces(int shopId, string nearPlaces);

        #endregion

        #region Policies

        string GetPolicies(int shopId);
        bool UpdatePolicies(int shopId, string policies);

        #endregion

        #region Payments

        bool UpdatePayments(int shopId, string selectedValues);

        #endregion

        #endregion
    }

    public class StoreShop : IStoreShop
    {
        private readonly string _connectionString;
        private RpsShop r;

        public StoreShop() : this("PfoodDBConnection")
        {

        }

        public StoreShop(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsShop(_connectionString);
        }

        public List<IdentityShop> GetByPage(IdentityShop filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityShop identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityShop identity)
        {
            return r.Update(identity);
        }

        public IdentityShop GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }

        public List<IdentityShop> GetList(string keyword)
        {
            return r.GetList(keyword);
        }

        public bool UpdateMap(int shopId, string latitude, string longitude)
        {
            return r.UpdateMap(shopId, latitude, longitude);
        }

        public int AssignToUser(int shopId, int userId, bool isOwner)
        {
            return r.AssignToUser(shopId, userId, isOwner);
        }

        public List<IdentityShop> GetAssigned(int userId)
        {
            return r.GetAssigned(userId);
        }

        public bool DeleteAssignedUser(int shopId, int userId)
        {
            return r.DeleteAssignedUser(shopId, userId);
        }

        public List<IdentityShopMember> GetAssignedMembers(int shopId)
        {
            return r.GetAssignedMembers(shopId);
        }

        public int AssignCategory(int shopId, int categoryId)
        {
            return r.AssignCategory(shopId, categoryId);
        }

        #region Meta data

        #region Images

        public bool AddNewImage(MetaShopImage identity)
        {
            return r.AddNewImage(identity);
        }

        public bool RemoveImage(string shopId)
        {
            return r.RemoveImage(shopId);
        }

        public List<MetaShopImage> GetListImage(int shopId)
        {
            return r.GetListImage(shopId);
        }

        #endregion

        #region Amenities

        public bool UpdateAmenities(int shopId, string selectedValues)
        {
            return r.UpdateAmenities(shopId, selectedValues);
        }

        #endregion

        #region NearPlaces

        public string GetNearPlaces(int shopId)
        {
            return r.GetNearPlaces(shopId);
        }

        public bool UpdateNearPlaces(int shopId, string nearPlaces)
        {
            return r.UpdateNearPlaces(shopId, nearPlaces);
        }

        #endregion

        #region Policies

        public string GetPolicies(int shopId)
        {
            return r.GetPolicies(shopId);
        }

        public bool UpdatePolicies(int shopId, string policies)
        {
            return r.UpdatePolicies(shopId, policies);
        }

        #endregion

        #region Payments

        public bool UpdatePayments(int shopId, string selectedValues)
        {
            return r.UpdatePayments(shopId, selectedValues);
        }

        #endregion

        #endregion
    }
}
