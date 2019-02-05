using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP_NET_MVC_KP_TestCamoIt_.Models
{
    public class MappingViewModel
    {
        public string FileName { get; set; }
        public IEnumerable<ColumnModel> Columns { get; set; }

        public IEnumerable<string> Parameters { get; set; }

    }
}