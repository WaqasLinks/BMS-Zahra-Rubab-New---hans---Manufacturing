using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;
using Component = MYBUSINESS.Models.Component;

namespace MYBUSINESS.Controllers
{
    public class MOController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private MOViewModel moViewModel = new MOViewModel();
        // GET: MOs
        public ActionResult Index()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(DAL.dbMOes.Where(x => x.Components.Count() == 0).ToList());
        }
        public ActionResult IndexBOM()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(DAL.dbMOes.Where(x=>x.Components.Count() > 0).ToList());
        }

        public ActionResult SearchData(string suppId)
        {
            if (suppId.Trim() == string.Empty)
            {

                return PartialView("_SelectedMOs", DAL.dbMOes.OrderBy(i => i.Id).ToList());

            }
            else
            {
                int intSuppId = Int32.Parse(suppId.Trim());

                IQueryable<MO> selectedMOs = null;
                //selectedMOs = db.MOs.Where(p => p.SupplierId == intSuppId);
                return PartialView("_SelectedMOs", selectedMOs.OrderBy(i => i.Id).ToList());

            }

        }


        public ActionResult Create()
        {
            //int maxId = db.MOs.Max(p => p.Id);
            decimal maxId = db.MOes.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;
            ViewBag.Suppliers = DAL.dbSuppliers;
            MO prod = new MO();

            prod.PurchasePrice = 0;
            prod.SalePrice = 0;
            prod.Stock = 0;

            prod.Saleable = true;
            moViewModel.MO = prod;
            moViewModel.MOes = DAL.dbMOes;
            return View(moViewModel);
        }

        // POST: MOs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //public ActionResult Create([Bind(Prefix = "Customer", Include = "Name,Address")] Customer Customer, [Bind(Prefix = "SaleOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,CustomerId,Remarks,Remarks2,PaymentMethod,PaymentDetail,SaleReturn,BankAccountId,Date")] SO sO, [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,SalePrice,Quantity,SaleType,PerPack,IsPack,Product")] List<SOD> sOD, FormCollection collection)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "MO", Include = "Id,Name,PurchasePrice,SalePrice,WholeSalePrice,Stock,Saleable,PerPack,IsService,ShowIn,BarCode,Category,Cylinder,Model,Dimension,OpenWeight,ClosedWeight,CoolantCapacity,VoltageRating,LubeOilCapacity,Width")] MO mo, [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<Component> components)
        {
            if (mo.Stock == null)
            {
                mo.Stock = 0;
            }

            if (mo.PerPack == null || mo.PerPack == 0)
            {
                mo.PerPack = 1;
            }

            mo.Stock = mo.Stock * mo.PerPack;
            
            if (ModelState.IsValid)
            {
                db.MOes.Add(mo);

                if (components !=null && components.Count > 0)
                {
                    foreach (var item in components) item.ParentProductId = mo.Id;
                    db.Components.AddRange(components);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(mo);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MO product = db.MOes.Find(id);
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








            moViewModel.MO = product;
            moViewModel.MOes = DAL.dbMOes;
            moViewModel.Components = db.Components.Where(x => x.ParentProductId == product.Id).ToList();

            return View(moViewModel);
            //return View(product);
        }
        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Prefix = "MO", Include = "Id,Name,PurchasePrice,SalePrice,WholeSalePrice,Stock,Saleable,PerPack,IsService,ShowIn,BarCode,Category,Cylinder,Model,Dimension,OpenWeight,ClosedWeight,CoolantCapacity,VoltageRating,LubeOilCapacity,Width")] MO mo, [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        //public ActionResult Edit([Bind(Include = "Id,Name,PurchasePrice,SalePrice,WholeSalePrice,Stock,Saleable,PerPack,IsService,ShowIn,BarCode")] Product product)
        {
            //Product prd = db.Products.Where(x => x.Id == product.Id).FirstOrDefault();
            //product.SuppId = prd.SuppId;
            if (mo.Stock == null)
            {
                mo.Stock = 0;
            }

            if (mo.PerPack == null || mo.PerPack == 0)
            {
                mo.PerPack = 1;
            }

            mo.Stock = mo.Stock * mo.PerPack;
            //del old subitems
            List<SubItem> delSubItems = db.SubItems.Where(x => x.ParentProductId == mo.Id).ToList();

            db.SubItems.RemoveRange(delSubItems);

            if (ModelState.IsValid)
            {
                db.Entry(mo).State = EntityState.Modified;

                if (subItems != null && subItems.Count > 0)
                {
                    foreach (var item in subItems) item.ParentProductId = mo.Id;
                    db.SubItems.AddRange(subItems);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(mo);
        }


        // GET: Products/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MO mo = db.MOes.Find(id);
            if (mo == null)
            {
                return HttpNotFound();
            }
            return View(mo);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MO product = db.MOes.Find(id);
            bool isPresent = false;
            if (db.PODs.FirstOrDefault(x => x.ProductId == id) != null || db.SODs.FirstOrDefault(x => x.ProductId == id) != null)
            {
                isPresent = true;
            }

            if (isPresent == false)
            {
                db.MOes.Remove(product);
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
