using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailConsoler.Extensions
{
    public static class ExceptionExtentions
    {
        public static Exception ToOriginalExeception(this Exception ex)
        {
            var tempEx = ex;
            while (tempEx.InnerException != null) tempEx = tempEx.InnerException;
            return tempEx;
        }
    }
}