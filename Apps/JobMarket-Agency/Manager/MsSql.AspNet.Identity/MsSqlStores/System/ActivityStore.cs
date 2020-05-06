﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using System.Data;
using System.Data.SqlClient;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity
{
    public class ActivityStore : IActivityStore
    {
        private readonly string _connectionString;
        private ActivityRepository _r;


        public ActivityStore()
            : this("DefaultConnection")
        {

        }

        public ActivityStore(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _r = new ActivityRepository(_connectionString);
        }

        #region Activity Log

        public bool WriteActivityLog(ActivityLog log)
        {
            return _r.InsertActivityLog(log);
        }

        public List<ActivityLog> GetActivityLogByUserId(string UserId, int page, int pageSize)
        {
            return _r.GetActivityLogByUserId(UserId, page, pageSize).ToList();
        }

        public int CountAllActivityLogByUserId(string UserId)
        {
            return _r.CountAllActivityLogByUserId(UserId);
        }

        public List<ActivityLog> FilterActivityLog(ActivityLogQueryParms filters)
        {
            return _r.FilterActivityLog(filters).ToList();
        }

        public int CountAllFilterActivityLog(ActivityLogQueryParms filters)
        {
            return _r.CountAllFilterActivityLog(filters);
        }

        public ActivityLog GetActivityLogById(string Id)
        {
            return _r.GetActivityLogById(Id);
        }

        #endregion
       
    }

    public interface IActivityStore
    {
        bool WriteActivityLog(ActivityLog log);
        List<ActivityLog> GetActivityLogByUserId(string UserId, int page, int pageSize);
        int CountAllActivityLogByUserId(string UserId);
        List<ActivityLog> FilterActivityLog(ActivityLogQueryParms filters);
        int CountAllFilterActivityLog(ActivityLogQueryParms filters);
        ActivityLog GetActivityLogById(string Id);
    }
}
