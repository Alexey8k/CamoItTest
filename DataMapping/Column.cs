using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using MappingConfigSection;

namespace DataMapping
{
    public class Column
    {
        public string Header { get; set; } = string.Empty;

        public string Parameter { get; set; }

        public List<string> ExampleValues { get; set; } = new List<string>();

        public List<string> DataErrors { get; set; } = new List<string>();
    }
}