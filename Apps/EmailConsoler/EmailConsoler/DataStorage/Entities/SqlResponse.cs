using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsoler.DataStorage
{
    /// <summary>
    /// Store the error status each time SP be executed.
    /// </summary>
    public class SqlResponse
    {
        public int RowsAffected { get;set;}
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }        
        public bool IsSuccess { get { return string.IsNullOrEmpty(ErrorMessage); } }
    }

    public class SqlDataSetResponse : SqlResponse
    {
        public DataSet DataSetResult { get; set; }
    }

    public class SqlDataTableResponse : SqlResponse
    {
        public DataTable DataTableResult { get; set; }
    }

    public class SqlObjectResponse : SqlResponse
    {
        public object ObjectResult { get; set; }
    }

    public static class SqlResponseExtensions
    {
        public static void FillBaseClass(this SqlResponse derivedObj, SqlResponse basedObj)
        {
            derivedObj.RowsAffected = basedObj.RowsAffected;
            derivedObj.ErrorCode = basedObj.ErrorCode;
            derivedObj.ErrorMessage = basedObj.ErrorMessage;            
        }
    }
}
