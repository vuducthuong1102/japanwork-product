using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsoler.Helpers
{
    public static class ExceptionExtensions
    {
        public static string GetInnerMessage(this Exception ex)
        {
            string strMessage = "";
            if (ex !=null)
            {
                var tempEx = ex;
                while(tempEx.InnerException!=null)
                {
                    tempEx = tempEx.InnerException;
                }

                strMessage = tempEx.Message;
            }

            return strMessage;
        }
    }
}
