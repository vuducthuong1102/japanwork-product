using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Diagnostics;

namespace MySite.Owin.Middlewares
{
    public interface ILogger
    {
        void Write(string message, params object[] args);
    }

    public class Logger : ILogger
    {
        public void Write(string message, params object[] args)
        {
            Debug.WriteLine(message, args);
        }
    }
}