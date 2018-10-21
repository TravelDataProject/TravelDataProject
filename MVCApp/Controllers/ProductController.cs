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

        private IEnumerable<ProdElement> GetAllProducts(int startIndex, int endIndex)
        {
            using (DataTable productsTable = BuildProductTable())
            {
                daoProduct.GetDataTableFromTable(productsTable, startIndex, endIndex);
                return ConvertDataTableToProdElement(productsTable);
            }
        }

        private IEnumerable<ProdElement> GetAllProducts(int? limit = null)
        {
            //return daoProduct.GetAll();
            using (DataTable productsTable = BuildProductTable())
            {
                daoProduct.GetDataTableFromTable(productsTable, limit);
                return ConvertDataTableToProdElement(productsTable);
            }
        }

        private DataTable BuildProductTable()
        {
            DataTable productsTable = new DataTable("Tous les produits");
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(ProdElement));
            PropertyDescriptorCollection propertiesOfObject = null;
            foreach (PropertyDescriptor prop in properties)
            {
                Type typ = prop.GetType();
                if (prop.PropertyType.IsSubclassOf(typeof(AbstractEntity)))
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    var obj = Activator.CreateInstance(prop.PropertyType);
                    propertiesOfObject = TypeDescriptor.GetProperties(obj.GetType());
                    foreach (PropertyDescriptor currentProp in propertiesOfObject)
                    {
                        productsTable.Columns.Add(prop.Name + "." + currentProp.Name, Nullable.GetUnderlyingType(currentProp.PropertyType) ?? currentProp.PropertyType);
                    }
                }
                else
                {
                    productsTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }
            productsTable.Columns.Add("merchantRef");
            return productsTable;
        }

        private IEnumerable<ProdElement> ConvertDataTableToProdElement(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                yield return new ProdElement
                {
                    lang = row["lang"].ToString(),
                    id = Convert.ToInt64(row["id"].ToString()),
                    web_offer = row["web_offer"].ToString(),
                    stock_quantity = row["stock_quantity"].ToString(),
                    pre_order = row["pre_order"].ToString(),
                    is_for_sale = row["is_for_sale"].ToString(),
                    in_stock = row["in_stock"].ToString(),
                    brand = row["brand"].ToString(),
                    cat = row["cat"].ToString(),
                    price = new PriceElement()
                    {
                        curr = row["price.curr"].ToString(),
                        buynow = double.Parse(row["price.buynow"].ToString().Replace(".", ",")),
                        rrp = double.Parse(row["price.rrp"].ToString().Replace(".", ",")),
                        store = double.Parse(row["price.store"].ToString().Replace(".", ",")),
                        basePrice = double.Parse(row["price.basePrice"].ToString().Replace(".", ",")),
                    },
                    text = new TextElement()
                    {
                        name = row["text.name"].ToString(),
                    },
                    uri = new UriElement()
                    {
                        awTrack = row["uri.awTrack"].ToString(),
                        alternateImageTwo = row["uri.alternateImageTwo"].ToString(),
                        alternateImageThree = row["uri.alternateImageThree"].ToString(),
                        awImage = row["uri.awImage"].ToString(),
                        awThumb = row["uri.awThumb"].ToString(),
                        mImage = row["uri.mImage"].ToString(),
                        mLink = row["uri.mLink"].ToString(),
                    },
                    vertical = new VerticalElement()
                    {
                        id = (int)row["vertical.id"],
                        name = row["vertical.name"].ToString(),
                        availability = (int)row["vertical.availability"],
                        departureDate = row["vertical.departureDate"].ToString(),
                        destinationCity = row["vertical.destinationCity"].ToString(),
                        destinationCountry = row["vertical.destinationCountry"].ToString(),
                        destinationRegion = row["vertical.destinationRegion"].ToString(),
                        destinationType = row["vertical.destinationType"].ToString(),
                        duration = (int)row["vertical.duration"],
                        hotelAddress = row["vertical.hotelAddress"].ToString(),
                        latitude = row["vertical.latitude"].ToString(),
                        longitude = row["vertical.longitude"].ToString(),
                        returnDate = row["vertical.returnDate"].ToString(),
                        roomType = row["vertical.roomType"].ToString(),
                        startingFromPrice = double.Parse(row["vertical.startingFromPrice"].ToString().Replace(".", ",")),
                        travelRating = row["vertical.travelRating"].ToString(),
                        travelType = row["vertical.travelType"].ToString(),
                    },
                    pId = row["pId"].ToString(),
                };
            }
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