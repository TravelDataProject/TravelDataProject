using DataAccess;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    public class ProductController : Controller
    {
        private DaoMerchant daoMerchant;
        private DaoProduct daoProduct;

        public ProductController()
        {
            daoMerchant = DaoMerchant.Instance;
            daoProduct = DaoProduct.Instance;
        }

        private IEnumerable<ProdElement> GetAllProducts(int? startIndex = null, int? endIndex = null)
        {
            return daoProduct.GetAll(startIndex, endIndex);
        }

        // GET: Product
        public ActionResult Index(int? page)
        {
            int pageSize = 25;
            int startIndex = (page != null && page != 1) ? (((page.Value - 1) * pageSize) + 1) : 1;
            int endIndex = startIndex == 1 ? pageSize : (page.Value * pageSize);
            int productListCount = daoProduct.GetCount();
            var productList = GetAllProducts(startIndex, endIndex);
            var pager = new Pager(productListCount, page, pageSize);

            var viewModel = new IndexViewModel<ProdElement>
            {
                Items = productList,
                Pager = pager
            };
            FreeMemory();
            return View(viewModel);
        }

        private void FreeMemory()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
            GC.WaitForPendingFinalizers();
        }

        //public FileResult Download(string id)
        public ActionResult Download(string id)
        {
            FreeMemory();

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            string DateNow = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            string TimeNow = DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
            string FileName = "JsonFile_" + DateNow + "_" + TimeNow + ".json";
            string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string FullPath = Path.Combine(DesktopPath, FileName);

            var productList = GetAllProducts();

            using (StreamWriter sw = new StreamWriter(FullPath))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, productList);
                }
            }

            FreeMemory();

            //return File(FullPath, "application/json", FileName);
            return Content("<script language='javascript' type='text/javascript'>alert('File succesfully generated in Desktop!');</script>");
            //return RedirectToAction("Index");
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            string DateNow = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            string TimeNow = DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
            string FileName = "JsonFile_" + DateNow + "_" + TimeNow + ".json";
            string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string FullPath = Path.Combine(DesktopPath, FileName);

            var productList = GetAllProducts();

            using (StreamWriter sw = new StreamWriter(FullPath))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, productList);
                }
            }
        }

        #region Other Controller Actions

        //// GET: Product/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Product/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Product/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Product/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Product/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Product/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Product/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        #endregion Other Controller Actions
    }
}