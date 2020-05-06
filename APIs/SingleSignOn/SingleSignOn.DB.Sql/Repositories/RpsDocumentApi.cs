using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SingleSignOn.ShareLibs.Exceptions;
using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.ShareLibs;
using Newtonsoft.Json;

namespace SingleSignOn.DB.Sql.Repositories
{
    public class RpsDocumentApi
    {
        private readonly string _connectionString;

        public RpsDocumentApi(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsDocumentApi()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SingleSignOnDB"].ConnectionString;
        }
        public void InsertDocumentApi(string linkUrl, string data)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@LinkUrl", linkUrl},
                    {"@Data", data}
            };

            try
            {
                var sqlCmd = @"Document_Api_Insert";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());

            }
            catch (Exception ex)
            {
                var strError = "Failed to Document_Api_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public IdentityDocumentApi GetByLinkUrl(string linkUrl)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@LinkUrl", linkUrl}
                };

            try
            {
                var sqlCmd = @"DocumentApi_GetByLinkUrl";

                var entity = (IdentityDocumentApi)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingDataFromReader, parameters.ToSqlParams());
                return entity;

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetUserById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        private IdentityDocumentApi ParsingDataFromReader(SqlDataReader reader)
        {
            if (reader.Read())
            {
                var entity = new IdentityDocumentApi();
                entity.LinkUrl = reader["LinkUrl"].ToString();
                entity.Data = reader["Data"].ToString();
              
                return entity;
            }

            return null;
        }
    }
}
