using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASP_NET_MVC_KP_TestCamoIt_.Models
{
    public class ColumnModel
    {
        [Display(Name = "Колонка")]
        public string Header { get; set; } = string.Empty;

        [Display(Name = "Параметр")]
        public string Parameter { get; set; } = string.Empty;

        [Display(Name = "Пример значений")]
        public List<string> ExampleValues { get; set; } = new List<string>();

        [Display(Name = "Ошибки данных")]
        public List<string> DataErrors { get; set; } = new List<string>();
    }
}