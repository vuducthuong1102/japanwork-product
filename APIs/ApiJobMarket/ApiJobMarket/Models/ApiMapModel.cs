using Newtonsoft.Json;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ApiJobMarket.Models
{
    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public double south { get; set; }
        public double west { get; set; }
        public double north { get; set; }
        public double east { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public Viewport viewport { get; set; }
    }

    public class AddressComponentLanguage
    {
        public string languagecode { get; set; }

        public ApiMapModel model { get; set; }
    }

    public class ApiMapModel
    {
        public List<AddressComponent> address_components { get; set; }
        public string adr_address { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        public string reference { get; set; }
        public string scope { get; set; }
        public List<string> types { get; set; }
        public string url { get; set; }
        public int utc_offset { get; set; }
        public string vicinity { get; set; }
        public List<object> html_attributions { get; set; }
        public string languages { get; set; }
    }

    public class ApiPlaceFilterModel
    {
        public int PlaceId { get; set; }
        public int ProvinceId { get; set; }
    }
}