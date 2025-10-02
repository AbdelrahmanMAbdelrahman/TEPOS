using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP
{
    public  class ProjectVersion
    {
        public static string Version ="v=2.0.0";
        static string _name = "2ndRelease";
        static DateTime _date = DateTime.Parse("2019-04-29");
        static readonly string SiteUrl= HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + '/';
        public static string Asset(string file)
        {
            return SiteUrl + file + "?v=" + Version;
        }
    }
}