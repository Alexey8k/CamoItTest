using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASP_NET_MVC_KP_TestCamoIt_.Auxiliary_Logic;
using ASP_NET_MVC_KP_TestCamoIt_.Mapping;
using ASP_NET_MVC_KP_TestCamoIt_.Models;
using AutoMapper;
using DataMapping;
using PagedList;
using PagedList.Mvc;

namespace ASP_NET_MVC_KP_TestCamoIt_.Controllers
{
    public class HomeController : Controller
    {
        private IMappingManager MappingManager
        {
            get => (MappingManager)Session["MappingManager"];
            set => Session["MappingManager"] = value;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UploadFileModel uploadFile)
        {
            DataSource _dataSource = null;
            try
            {
                _dataSource = new CsvDataSource(uploadFile.File);

            }
            catch (DuplicateNameException)
            {
                ModelState.AddModelError("File", "Файл содержит дублируещиеся названия колонок");
            }
            catch (DataException e)
            {
                ModelState.AddModelError("File", e.Message);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("File", e.Message);
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            MappingManager = new MappingManager(((CsvDataSource)_dataSource).DataList, uploadFile.FileName);

            return View("Mapping", ((MappingManager)MappingManager).Mapping());
        }

        [HttpPost]
        public ActionResult Mapping(FormCollection collection)
        {
            var parameters = collection.AllKeys.ToDictionary(key => key, key => collection[key]);
            ((MappingManager)MappingManager).SetParameters(parameters);
            var errorPapameters = ((MappingManager)MappingManager).ValidationParameters();
            foreach (var error in errorPapameters)
            {
                ModelState.AddModelError("Columns", error);
            }

            if (!ModelState.IsValid || !((MappingManager)MappingManager).IsValidationData())
                return View(((MappingManager)MappingManager).Mapping());

            try
            {
                //((MappingManager)MappingManager).CreateDb();
                ((MappingManager)MappingManager).CopyDataToDb();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Columns", e.Message);
                return View(((MappingManager)MappingManager).Mapping());
            }

            return View("DataView", 1);
        }

        public ActionResult DataView(int? page)
        {
            return View(page ?? 1);
        }

        public ActionResult GetDataView(int? page)
        {
            return PartialView(Pagination(page ?? 1, 50));
        }

        private IPagedList<DataRow> Pagination(int pageNumber, int pageSize)
            => ((MappingManager)MappingManager).ViewDataTable.AsEnumerable().ToPagedList(pageNumber, pageSize);
    }
}