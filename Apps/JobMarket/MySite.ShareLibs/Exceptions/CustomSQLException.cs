using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySite.ShareLibs.Exceptions
{
    public class CustomSQLException : Exception
    {
        public CustomSQLException()
            : base()
        {
        }

        public CustomSQLException(string key, string value)
            : base()
        {
            base.Data.Add(key, value);
        }

        public CustomSQLException(string message)
            : base(message)
        {
        }

        public CustomSQLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
