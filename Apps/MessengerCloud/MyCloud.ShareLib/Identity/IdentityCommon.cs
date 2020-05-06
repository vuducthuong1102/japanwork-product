using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCloud.SharedLib
{
    public class BaseOnlineUserIdentity
    {
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Avatar { get; set; }
        public virtual int UserObjectType { get; set; }
    }

    public class IdentityCommon
    {
        public string Keyword { get; set; }

        //[JsonIgnore]
        public int TotalCount { get; set; }

        [JsonIgnore]
        public DateTime? FromDate { get; set; }

        [JsonIgnore]
        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SortField { get; set; }

        //ASC or DESC
        public string SortType { get; set; }
    }
}
