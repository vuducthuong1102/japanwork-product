using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsGoogleMap
    {
        private readonly string _connectionString;

        public RpsGoogleMap(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsGoogleMap()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        public int Insert(IdentityGoogleMap identity)
        {
            var code = 0;
            var sqlCmd = @"GoogleMap_Insert";
           
            var parameters = new Dictionary<string, object>
            {
                {"@address_components", identity.address_components},
                {"@adr_address", identity.adr_address},
                {"@formatted_address", identity.formatted_address},
                {"@formatted_phone_number", identity.formatted_phone_number},
                {"@geometry", identity.geometry},
                {"@icon", identity.icon},
                {"@gid", identity.gid},
                {"@international_phone_number", identity.international_phone_number},
                {"@name", identity.name},
                {"@place_id", identity.place_id},
                {"@rating", identity.rating},
                {"@reference", identity.reference},
                {"@reviews", identity.reviews},
                {"@scope", identity.scope},
                {"@types", identity.types},
                {"@url", identity.url},
                {"@utc_offset", identity.utc_offset},
                {"@vicinity", identity.vicinity},
                {"@website", identity.website}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        code = Convert.ToInt32(reader[0]);
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to GoogleMap_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return code;
        }
    }
}
