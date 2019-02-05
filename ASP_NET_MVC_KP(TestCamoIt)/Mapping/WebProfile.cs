using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASP_NET_MVC_KP_TestCamoIt_.Models;
using AutoMapper;
using DataMapping;

namespace ASP_NET_MVC_KP_TestCamoIt_.Mapping
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<Column, ColumnModel>();
            CreateMap<MappingManager, MappingViewModel>();
        }
    }
}