﻿using MySite.Attributes;
using System.Web;
using System.Web.Mvc;

namespace MySite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
