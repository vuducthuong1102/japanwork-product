using ApiJobMarket.DB.Sql.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Manager.WebApp.Models
{
    public class ApiFooterModel
    {
        public string BodyContent { get; set; }
        public string LanguageCode { get; set; }
    }
}