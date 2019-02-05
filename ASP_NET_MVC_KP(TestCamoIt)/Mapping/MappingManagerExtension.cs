using System;
using System.Linq;
using System.Web;
using ASP_NET_MVC_KP_TestCamoIt_.Models;
using AutoMapper;
using DataMapping;

namespace ASP_NET_MVC_KP_TestCamoIt_.Mapping
{
    public static class MappingManagerExtension
    {
        public static MappingViewModel Mapping(this MappingManager obj)
        {
            return Mapper.Map<MappingManager, MappingViewModel>(obj);
        }
    }
}