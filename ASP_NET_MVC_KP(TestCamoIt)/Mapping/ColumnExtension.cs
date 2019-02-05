using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASP_NET_MVC_KP_TestCamoIt_.Controllers;
using ASP_NET_MVC_KP_TestCamoIt_.Models;
using AutoMapper;
using DataMapping;

namespace ASP_NET_MVC_KP_TestCamoIt_.Mapping
{
    public static class ColumnExtension
    {
        public static IEnumerable<ColumnModel> Mapping(this IEnumerable<Column> obj)
        {
            return Mapper.Map<IEnumerable<Column>, IEnumerable<ColumnModel>>(obj);
        }
    }
}