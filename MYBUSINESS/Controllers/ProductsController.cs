using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class ProductsController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private ProductViewModel productViewModel = new ProductViewModel();
        // GET: Products

        public ActionResult Index()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(DAL.dbProducts.Where(x => x.SubItems.Count() == 0).ToList());
        }
        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(DAL.dbProducts.Where(x => x.SubItems.Count() > 0).ToList());
            
        }
        
        

        public ActionResult SearchData(string suppId)
        {
            if (suppId.Trim() == string.Empty)
            {

                return PartialView("_SelectedProducts", DAL.dbProducts.OrderBy(i => i.Id).ToList());

            }
            else
            {
                int intSuppId = Int32.Parse(suppId.Trim());

                IQueryable<Product> selectedProducts = null;
                //selectedProducts = db.Products.Where(p => p.SupplierId == intSuppId);
                return PartialView("_SelectedProducts", selectedProducts.OrderBy(i => i.Id).ToList());

            }

        }


        public ActionResult Create()
        {
            //int maxId = db.Products.Max(p => p.Id);
            decimal maxId = db.Products.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;
            ViewBag.Suppliers = DAL.dbSuppliers;
            Product prod = new Product();

            prod.PurchasePrice = 0;
            prod.SalePrice = 0;
            prod.Stock = 0;

            prod.Saleable = true;
            productViewModel.Product = prod;
            productViewModel.Products = DAL.dbProducts;
            return View(productViewModel);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //public ActionResult Create([Bind(Prefix = "Customer", Include = "Name,Address")] Customer Customer, [Bind(Prefix = "SaleOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,CustomerId,Remarks,Remarks2,PaymentMethod,PaymentDetail,SaleReturn,BankAccountId,Date")] SO sO, [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,SalePrice,Quantity,SaleType,PerPack,IsPack,Product")] List<SOD> sOD, FormCollection collection)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "Product", Include = "Id,Name,PurchasePrice,SalePrice,WholeSalePrice,Stock,Saleable,PerPack,IsService,ShowIn,BarCode,Category,Cylinder,Model,Dimension,OpenWeight,ClosedWeight,CoolantCapacity,VoltageRating,LubeOilCapacity,Width")] Product product, [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        {
            if (product.Stock == null)
            {
                product.Stock = 0;
            }
            if (product.StockDefective == null)
            {
                product.Stock = 0;
            }
            if (product.PerPack == null || product.PerPack == 0)
            {
                product.PerPack = 1;
            }

            product.Stock = product.Stock * product.PerPack;
            
            if (ModelState.IsValid)
            {
                db.Products.Add(product);

                if (subItems !=null && subItems.Count > 0)
                {
                    foreach (var item in subItems) item.ParentProductId = product.Id;
                    db.SubItems.AddRange(subItems);
                    product.HasSubItems = true;
                }
                else
                {
                    product.HasSubItems = false;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            product.Stock = product.Stock / product.PerPack;
            //ViewBag.SuppName = product.Supplier.Name;
            if (product == null)
            {
                return HttpNotFound();
            }
            List<MyUnitType> myUnitTypeOptionList = new List<MyUnitType> {
                            new MyUnitType {
                                Text = "Product",
                                Value = "false"
                            },
                            new MyUnitType {
                                Text = "Service",
                                Value = "true"
                            }
                        };
            ViewBag.UnitTypeOptionList = myUnitTypeOptionList;

            List<MyCategory> MyCategory = new List<MyCategory> {
                            new MyCategory {
                                Text = "Engine",
                                Value = "Engine"
                            },
                            new MyCategory {
                                Text = "Parts",
                                Value = "Parts"
                            },
                            new MyCategory {
                                Text = "Alternator",
                                Value = "Alternator"
                            },
                            new MyCategory {
                                Text = "Generator",
                                Value = "Generator"
                            }
                        };
            ViewBag.MyCategory = MyCategory;

            List<MyDimension> MyDimension = new List<MyDimension> {
                            new MyDimension {
                                Text = "Open",
                                Value = "Open"
                            },
                            new MyDimension {
                                Text = "Closed",
                                Value = "Closed"
                            }
                        };
            ViewBag.MyDimension = MyDimension;

            List<MyWeight> MyWeight = new List<MyWeight> {
                            new MyWeight {
                                Text = "Open",
                                Value = "Open"
                            },
                            new MyWeight {
                                Text = "Closed",
                                Value = "Closed"
                            }
                        };
            ViewBag.MyWeight = MyWeight;








            productViewModel.Product = product;
            productViewModel.Products = DAL.dbProducts;
            productViewModel.SubItems = db.SubItems.Where(x => x.ParentProductId == product.Id).ToList();

            return View(productViewModel);
            //return View(product);
        }
        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Prefix = "Product", Include = "Id,Name,PurchasePrice,SalePrice,WholeSalePrice,Stock,Saleable,PerPack,IsService,ShowIn,BarCode,Category,Cylinder,Model,Dimension,OpenWeight,ClosedWeight,CoolantCapacity,VoltageRating,LubeOilCapacity,Width")] Product product, [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        //public ActionResult Edit([Bind(Include = "Id,Name,PurchasePrice,SalePrice,WholeSalePrice,Stock,Saleable,PerPack,IsService,ShowIn,BarCode")] Product product)
        {
            //Product prd = db.Products.Where(x => x.Id == product.Id).FirstOrDefault();
            //product.SuppId = prd.SuppId;
            if (product.Stock == null)
            {
                product.Stock = 0;
            }

            if (product.PerPack == null || product.PerPack == 0)
            {
                product.PerPack = 1;
            }

            product.Stock = product.Stock * product.PerPack;
            //del old subitems
            List<SubItem> delSubItems = db.SubItems.Where(x => x.ParentProductId == product.Id).ToList();

            db.SubItems.RemoveRange(delSubItems);

            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;

                if (subItems != null && subItems.Count > 0)
                {
                    foreach (var item in subItems) item.ParentProductId = product.Id;
                    db.SubItems.AddRange(subItems);
                    product.HasSubItems = true;
                }
                else
                {
                    product.HasSubItems = false;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(product);
        }


        public ActionResult CreateService()
        {
            //int maxId = db.Products.Max(p => p.Id);
            decimal maxId = db.Products.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;
            ViewBag.Suppliers = DAL.dbSuppliers;
            Product prod = new Product();

            prod.PurchasePrice = 0;
            prod.SalePrice = 0;
            prod.Stock = 0;

            prod.Saleable = true;
            return View(prod);
        }

        public ActionResult EditService(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            product.Stock = product.Stock / product.PerPack;
            //ViewBag.SuppName = product.Supplier.Name;
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Product product = db.Products.Find(id);
            bool isPresent = false;
            if (db.PODs.FirstOrDefault(x => x.ProductId == id) != null || db.SODs.FirstOrDefault(x => x.ProductId == id) != null)
            {
                isPresent = true;
            }

            if (isPresent == false)
            {
                db.Products.Remove(product);
            }
            else
            {
                product.Status = "D";
                db.Entry(product).Property(x => x.Status).IsModified = true;

            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
