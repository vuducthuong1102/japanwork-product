using Manager.SharedLibs;
using Manager.SharedLibs;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsProduct
    {
        private readonly string _connectionString;

        public RpsProduct(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsProduct()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }


        #region  Common

        public List<IdentityProduct> GetByPage(IdentityProduct filter, int currentPage, int pageSize)
        {
            //Common syntax
            var sqlCmd = @"Product_GetByPage";
            List<IdentityProduct> listData = null;

            //For paging
            int offset = (currentPage - 1) * pageSize;

            //var propertiesFilter = (filter.SelectedProperties != null && filter.SelectedProperties.Length > 0)? string.Join(",", filter.SelectedProperties) : string.Empty;
            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Keyword", filter.Keyword },
                    {"@Status", filter.Status },
                    //{"@ProviderId", filter.ProviderId },
                    //{"@ProductCategoryId", filter.ProductCategoryId },
                    {"@PropertyCategoryId", filter.PropertyCategoryId },
                    {"@PropertyList", filter.PropertyList },
                    {"@Offset", offset },
                    {"@PageSize", pageSize }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityProduct> GetByListIds(List<int> listIds)
        {
            //Common syntax
            var sqlCmd = @"Product_GetByListIds";
            List<IdentityProduct> listData = null;
           
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ListIds", string.Join(",", listIds) }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetByListIds. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityProduct> GetActiveForChoosen(IdentityProduct filter, int currentPage, int pageSize)
        {
            //Common syntax
            var sqlCmd = @"Product_GetActiveForChoosen";
            List<IdentityProduct> listData = null;

            var propertiesFilter = (filter.SelectedProperties != null && filter.SelectedProperties.Length > 0) ? string.Join(",", filter.SelectedProperties) : string.Empty;
            //For paging
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword },
                //{"@ProviderId", filter.ProviderId },
                //{"@ProductCategoryId", filter.ProductCategoryId },
                {"@PropertyCategoryId", filter.PropertyCategoryId },
                {"@PropertyList", propertiesFilter },
                {"@Offset", offset },
                {"@PageSize", pageSize }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetActiveForChoosen. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityProduct> GetListNeedReflectByPage(IdentityProduct filter, int currentPage, int pageSize)
        {
            //Common syntax
            var sqlCmd = @"Product_GetListNeedReflectByPage";
            List<IdentityProduct> listData = null;

            var propertiesFilter = (filter.SelectedProperties != null && filter.SelectedProperties.Length > 0) ? string.Join(",", filter.SelectedProperties) : string.Empty;
            //For paging
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword },
                {"@PropertyCategoryId", filter.PropertyCategoryId },
                {"@PropertyList", propertiesFilter },
                {"@Offset", offset },
                {"@PageSize", pageSize }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListFromReader(reader);

                        if (reader.NextResult())
                        {
                            var listWarehouseInfo = new List<IdentityWarehouse>();
                            while (reader.Read())
                            {
                                var warehouseInfo = new RpsWarehouse().ExtractWarehouseData(reader);

                                listWarehouseInfo.Add(warehouseInfo);
                            }

                            if(listData != null && listData.Count > 0)
                            {
                                if(listWarehouseInfo != null && listWarehouseInfo.Count > 0)
                                {
                                    foreach (var item in listData)
                                    {
                                        var matchedInfo = listWarehouseInfo.Where(x => x.ProductId == item.Id).FirstOrDefault();
                                        if(matchedInfo != null)
                                        {
                                            item.StockTakeQTY = matchedInfo.StockTakeQTY;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetListNeedReflectByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public int Insert(IdentityProduct identity)
        {
            //Common syntax
            var sqlCmd = @"Product_Insert";
            var newId = 0;
            StringBuilder propertyInsertCmd = new StringBuilder();

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Code", identity.Code },
                    {"@ProductCategoryId", identity.ProductCategoryId },
                    {"@ProviderId", identity.ProviderId },
                    {"@Name", identity.Name },
                    //{"@ShortDescription", identity.ShortDescription },
                    //{"@Detail", identity.Detail },
                    //{"@OtherInfo", identity.OtherInfo },
                    //{"@Cost", identity.Cost },
                    //{"@SaleOffCost", identity.SaleOffCost },
                    //{"@CurrencyId", identity.CurrencyId },
                    {"@MinInventory", identity.MinInventory },
                    {"@UnitId", identity.UnitId },
                    {"@CreatedBy", identity.CreatedBy },
                    //{"@CreatedDate", identity.CreatedDate },
                    //{"@LastUpdatedBy", identity.LastUpdatedBy },
                    //{"@LastUpdated", identity.LastUpdated },
                    {"@Status", identity.Status },
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = Convert.ToInt32(returnObj);

                    if(newId > 0)
                    {
                        if(identity.Properties != null && identity.Properties.Count > 0)
                        {
                            //propertyInsertCmd.Append(string.Format(@"DELETE FROM tbl_product_property WHERE 1=1 AND ProductId = {0}; ", newId));

                            foreach (var item in identity.Properties)
                            {
                                if(item.PropertyId > 0)
                                {
                                    var propertyCmd = string.Format(@"INSERT INTO tbl_product_property (ProductId,PropertyCategoryId,PropertyId) 
                                    VALUES ({0},{1},{2}); ", newId, item.PropertyCategoryId, item.PropertyId);

                                    propertyInsertCmd.Append(propertyCmd);
                                }                                
                            }                            
                        }

                        var propertyInsertCmdCmdExec = propertyInsertCmd.ToString();
                        if (!string.IsNullOrEmpty(propertyInsertCmdCmdExec))
                        {
                            //Insert properties
                            MsSqlHelper.ExecuteScalar(conn, CommandType.Text, propertyInsertCmdCmdExec, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityProduct identity)
        {
            //Common syntax
            var sqlCmd = @"Product_Update";
            StringBuilder propertyInsertCmd = new StringBuilder();

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", identity.Id },
                    {"@Code", identity.Code },
                    {"@ProductCategoryId", identity.ProductCategoryId },
                    {"@ProviderId", identity.ProviderId },
                    {"@Name", identity.Name },
                    //{"@ShortDescription", identity.ShortDescription },
                    //{"@Detail", identity.Detail },
                    //{"@OtherInfo", identity.OtherInfo },
                    //{"@Cost", identity.Cost },
                    //{"@SaleOffCost", identity.SaleOffCost },
                    //{"@CurrencyId", identity.CurrencyId },
                    {"@MinInventory", identity.MinInventory },
                    {"@UnitId", identity.UnitId },
                    {"@LastUpdatedBy", identity.LastUpdatedBy },
                    {"@Status", identity.Status }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    if (identity.Id > 0)
                    {
                        if (identity.Properties != null && identity.Properties.Count > 0)
                        {
                            propertyInsertCmd.Append(string.Format(@"DELETE FROM tbl_product_property WHERE 1=1 AND ProductId = {0}; ", identity.Id));

                            foreach (var item in identity.Properties)
                            {
                                if (item.PropertyId > 0)
                                {
                                    var propertyCmd = string.Format(@"INSERT INTO tbl_product_property (ProductId,PropertyCategoryId,PropertyId) 
                                    VALUES ({0},{1},{2}); ", identity.Id, item.PropertyCategoryId, item.PropertyId);

                                    propertyInsertCmd.Append(propertyCmd);
                                }
                            }
                        }

                        var propertyInsertCmdCmdExec = propertyInsertCmd.ToString();
                        if (!string.IsNullOrEmpty(propertyInsertCmdCmdExec))
                        {
                            //Insert properties
                            MsSqlHelper.ExecuteScalar(conn, CommandType.Text, propertyInsertCmdCmdExec, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityProduct GetById(int id)
        {
            //Common syntax
            var sqlCmd = @"Product_GetById";
            IdentityProduct info = null;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", id }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractProductData(reader);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var property = ExtractProductPropertyData(reader);
                                info.Properties.Add(property);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public IdentityProduct GetDetail(int id)
        {
            //Common syntax
            var sqlCmd = @"Product_GetDetail";
            IdentityProduct info = null;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", id }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractProductData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public IdentityProduct GetByCode(string code)
        {
            //Common syntax
            var sqlCmd = @"Product_GetByCode";
            IdentityProduct info = null;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Code", code }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractProductData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_GetByCode. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public bool Delete(int id)
        {
            //Common syntax
            var sqlCmd = @"Product_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", id }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Product_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        #endregion

        #region  Helpers

        private List<IdentityProduct> ParsingListFromReader(IDataReader reader)
        {
            List<IdentityProduct> listData = listData = new List<IdentityProduct>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractProductData(reader);

                //Extends information
                if(reader.HasColumn("TotalCount"))
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }
            return listData;
        }

        public static IdentityProduct ExtractProductData(IDataReader reader)
        {
            var record = new IdentityProduct();

            //Seperate properties;
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Code = reader["Code"].ToString();
            //record.ProductCategoryId = Utils.ConvertToInt32(reader["ProductCategoryId"]);
            //record.ProviderId = Utils.ConvertToInt32(reader["ProviderId"]);
            record.Name = reader["Name"].ToString();
            //record.ShortDescription = reader["ShortDescription"].ToString();
            //record.Detail = reader["Detail"].ToString();
            //record.OtherInfo = reader["OtherInfo"].ToString();
            //record.Cost = reader["Cost"].ToString();
            //record.SaleOffCost = reader["SaleOffCost"].ToString();
            //record.UnitId = Utils.ConvertToInt32(reader["UnitId"]);
            //record.CurrencyId = Utils.ConvertToInt32(reader["CurrencyId"]);  
            record.MinInventory = Utils.ConvertToDouble(reader["MinInventory"]);
            record.UnitId = Utils.ConvertToInt32(reader["UnitId"]);
            //record.CreatedBy = Utils.ConvertToInt32(reader["CreatedBy"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            //record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
            //record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            if (reader.HasColumn("WarehouseNum"))
                record.WarehouseNum = Utils.ConvertToDouble(reader["WarehouseNum"]);

            return record;
        }


        public static IdentityProductProperty ExtractProductPropertyData(IDataReader reader)
        {
            var record = new IdentityProductProperty();

            //Seperate properties;
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.ProductId = Utils.ConvertToInt32(reader["ProductId"]);
            record.PropertyCategoryId = Utils.ConvertToInt32(reader["PropertyCategoryId"]);
            record.PropertyId = Utils.ConvertToInt32(reader["PropertyId"]);
           
            return record;
        }
        #endregion
    }

}
