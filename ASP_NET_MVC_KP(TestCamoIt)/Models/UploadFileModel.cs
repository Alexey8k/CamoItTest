using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;

namespace ASP_NET_MVC_KP_TestCamoIt_.Models
{
    public class UploadFileModel
    {
        //[RegularExpression(@".+\.(csv)$", ErrorMessage = "Не допустимый тип файла")]
        [Required(ErrorMessage = "Нет выбраного файла")]
        [Display(Name = "Выбрать...")]
        public HttpPostedFileBase File { get; set; }

        [RegularExpression(@"^[^\\/:\*\?<>|""]+\.(csv)$", ErrorMessage = "Не допустимый тип файла")]
        public string FileName { get; set; }
    }
}