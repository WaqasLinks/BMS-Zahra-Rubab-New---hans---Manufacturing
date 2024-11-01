using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MYBUSINESS.Models;
using MYBUSINESS.CustomClasses;

namespace MYBUSINESS.Controllers
{
    public class MOesController : Controller
    {
        private BusinessContext db = new BusinessContext();

        // GET: MOes
        public async Task<ActionResult> Index()
        {
            var mOes = db.MOes.Include(m => m.Product);
            return View(await mOes.ToListAsync());
        }

        // GET: MOes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MO mO = await db.MOes.FindAsync(id);
            if (mO == null)
            {
                return HttpNotFound();
            }
            return View(mO);
        }

        // GET: MOes/Create
        public ActionResult Create()
        {
            decimal maxId = db.MOes.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            return View();
        }

        // POST: MOes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,ProductId,StartDate,EndDate,Qty,Notes,CreateDate,UpdateDate,Status")] MO mO)
        {
            if (ModelState.IsValid)
            {
                mO.CreateDate = DateTime.Now;
                db.MOes.Add(mO);

                //--------------------
                
                Product product = db.Products.FirstOrDefault(x => x.Id == mO.ProductId);
                
                decimal StockInDB = (decimal)product.Stock; //(decimal)db.Products.AsNoTracking().FirstOrDefault(x => x.Id == mO.ProductId).Stock;

                string StatusInDB = "0";
                //means will add qty
                if (StatusInDB != "1" && mO.Status == "1")
                {
                    decimal maxId1 = (int)db.POes.DefaultIfEmpty().Max(p => p == null ? 0 : p.POSerial);
                    maxId1 += 1;
                    Employee emp = (Employee)Session["CurrentUser"];
                    decimal EmployeeId = emp.Id;

                    PO pO = new PO { Id = System.Guid.NewGuid().ToString().ToUpper(), POSerial = maxId1, BillAmount = 0, BillPaid = 0, Discount = 0, Balance = 0, PrevBalance = 0, Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")), PurchaseReturn = false, SupplierId = -1, PurchaseOrderAmount = 0, PurchaseOrderQty = mO.Qty, PaymentMethod = "Cash", EmployeeId = EmployeeId, BankAccountId = "1" };
                    db.POes.Add(pO);

                    POD pOD = new POD { POId = pO.Id, PODId = 1, ProductId = product.Id, OpeningStock = StockInDB, Quantity = mO.Qty, PurchasePrice = 0, PerPack = 1, IsPack = true, SaleType = false };
                    db.PODs.Add(pOD);
                    //------------
                    product.Stock += mO.Qty;
                    db.Entry(product).Property(x => x.Stock).IsModified = true;
                    List<SubItem> subItems = db.SubItems.Where(x => x.ParentProductId == product.Id).ToList();
                    if (subItems != null && subItems.Count > 0)
                    {
                        foreach (SubItem subItem in subItems)
                        {
                            Product component = db.Products.FirstOrDefault(x => x.Id == subItem.ProductId);
                            component.Stock -= subItem.Quantity * mO.Qty;
                            db.Entry(component).Property(x => x.Stock).IsModified = true;
                        }
                    }
                    
                }

                if (StatusInDB == "1" && mO.Status != "1")
                {
                    decimal maxId1 = (int)db.POes.DefaultIfEmpty().Max(p => p == null ? 0 : p.POSerial);
                    maxId1 += 1;
                    Employee emp = (Employee)Session["CurrentUser"];
                    decimal EmployeeId = emp.Id;

                    PO pO = new PO { Id = System.Guid.NewGuid().ToString().ToUpper(), POSerial = maxId1, BillAmount = 0, BillPaid = 0, Discount = 0, Balance = 0, PrevBalance = 0, Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")), PurchaseReturn = true, SupplierId = -1, PurchaseOrderAmount = 0, PurchaseOrderQty = (mO.Qty), PaymentMethod = "Cash", EmployeeId = EmployeeId, BankAccountId = "1" };
                    db.POes.Add(pO);

                    POD pOD = new POD { POId = pO.Id, PODId = 1, ProductId = product.Id, OpeningStock = StockInDB, Quantity = mO.Qty, PurchasePrice = 0, PerPack = 1, IsPack = true, SaleType = true };
                    db.PODs.Add(pOD);
                    //------------
                    product.Stock -= mO.Qty;
                    db.Entry(product).Property(x => x.Stock).IsModified = true;
                    List<SubItem> subItems = db.SubItems.Where(x => x.ParentProductId == product.Id).ToList();
                    if (subItems != null && subItems.Count > 0)
                    {
                        foreach (SubItem subItem in subItems)
                        {
                            Product component = db.Products.FirstOrDefault(x => x.Id == subItem.ProductId);
                            component.Stock += subItem.Quantity * mO.Qty; ;
                            db.Entry(component).Property(x => x.Stock).IsModified = true;
                        }
                    }
                    
                }




                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", mO.ProductId);
            return View(mO);
        }

        // GET: MOes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MO mO = await db.MOes.FindAsync(id);
            if (mO == null)
            {
                return HttpNotFound();
            }
            List<TextValuePair> StatusOptionLst = new List<TextValuePair> {
                            new TextValuePair {
                                Text = "InProgress",
                                Value = "0"
                            },
                            new TextValuePair {
                                Text = "Others",
                                Value = "2"
                            },
                            new TextValuePair {
                                Text = "Complete",
                                Value = "1"
                            }
                        };

            ViewBag.StatusOptionLst = StatusOptionLst;
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", mO.ProductId);
            ViewBag.StatusInDB = db.MOes.AsNoTracking().FirstOrDefault(x => x.Id == mO.Id).Status;
            return View(mO);
        }

        // POST: MOes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,ProductId,StartDate,EndDate,Qty,Notes,CreateDate,UpdateDate,Status")] MO mO)
        {

            if (ModelState.IsValid)
            {
                
                mO.UpdateDate = DateTime.Now;
                db.Entry(mO).State = EntityState.Modified;
                db.Entry(mO).Property(x => x.CreateDate).IsModified = false;
                //--------------------

                Product product = db.Products.FirstOrDefault(x => x.Id == mO.ProductId);

                decimal StockInDB = (decimal)product.Stock; //(decimal)db.Products.AsNoTracking().FirstOrDefault(x => x.Id == mO.ProductId).Stock;

                string StatusInDB = db.MOes.AsNoTracking().FirstOrDefault(x => x.Id == mO.Id).Status;
                //means will add qty
                if (StatusInDB != "1" && mO.Status == "1")
                {
                    decimal maxId1 = (int)db.POes.DefaultIfEmpty().Max(p => p == null ? 0 : p.POSerial);
                    maxId1 += 1;
                    Employee emp = (Employee)Session["CurrentUser"];
                    decimal EmployeeId = emp.Id;

                    PO pO = new PO { Id = System.Guid.NewGuid().ToString().ToUpper(), POSerial = maxId1, BillAmount = 0, BillPaid = 0, Discount = 0, Balance = 0, PrevBalance = 0, Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")), PurchaseReturn = false, SupplierId = -1, PurchaseOrderAmount = 0, PurchaseOrderQty = mO.Qty, PaymentMethod = "Cash", EmployeeId = EmployeeId, BankAccountId = "1" };
                    db.POes.Add(pO);

                    POD pOD = new POD { POId = pO.Id, PODId = 1, ProductId = product.Id, OpeningStock = StockInDB, Quantity = mO.Qty, PurchasePrice = 0, PerPack = 1, IsPack = true, SaleType = false };
                    db.PODs.Add(pOD);
                    //------------
                    product.Stock += mO.Qty;
                    db.Entry(product).Property(x => x.Stock).IsModified = true;
                    List<SubItem> subItems = db.SubItems.Where(x => x.ParentProductId == product.Id).ToList();
                    if (subItems != null && subItems.Count > 0)
                    {
                        foreach (SubItem subItem in subItems)
                        {
                            Product component = db.Products.FirstOrDefault(x => x.Id == subItem.ProductId);
                            component.Stock -= subItem.Quantity * mO.Qty; ;
                            db.Entry(component).Property(x => x.Stock).IsModified = true;
                        }
                    }
                    
                }

                if (StatusInDB == "1" && mO.Status != "1")
                {
                    decimal maxId1 = (int)db.POes.DefaultIfEmpty().Max(p => p == null ? 0 : p.POSerial);
                    maxId1 += 1;
                    Employee emp = (Employee)Session["CurrentUser"];
                    decimal EmployeeId = emp.Id;

                    PO pO = new PO { Id = System.Guid.NewGuid().ToString().ToUpper(), POSerial = maxId1, BillAmount = 0, BillPaid = 0, Discount = 0, Balance = 0, PrevBalance = 0, Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")), PurchaseReturn = true, SupplierId = -1, PurchaseOrderAmount = 0, PurchaseOrderQty = (mO.Qty), PaymentMethod = "Cash", EmployeeId = EmployeeId, BankAccountId = "1" };
                    db.POes.Add(pO);

                    POD pOD = new POD { POId = pO.Id, PODId = 1, ProductId = product.Id, OpeningStock = StockInDB, Quantity = mO.Qty, PurchasePrice = 0, PerPack = 1, IsPack = true, SaleType = true };
                    db.PODs.Add(pOD);
                    //------------
                    product.Stock -= mO.Qty;
                    db.Entry(product).Property(x => x.Stock).IsModified = true;
                    List<SubItem> subItems = db.SubItems.Where(x => x.ParentProductId == product.Id).ToList();
                    if (subItems != null && subItems.Count > 0)
                    {
                        foreach (SubItem subItem in subItems)
                        {
                            Product component = db.Products.FirstOrDefault(x => x.Id == subItem.ProductId);
                            component.Stock += subItem.Quantity * mO.Qty; ;
                            db.Entry(component).Property(x => x.Stock).IsModified = true;
                        }
                    }
                    
                }

                //db.Entry(product).State = EntityState.Modified;


                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", mO.ProductId);
            return View(mO);
        }

        // GET: MOes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MO mO = await db.MOes.FindAsync(id);
            if (mO == null)
            {
                return HttpNotFound();
            }
            return View(mO);
        }

        // POST: MOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MO mO = db.MOes.Find(id);
            bool isCompleted = false;
            if (mO.Status == "1")
            {
                isCompleted = true;
            }

            if (isCompleted == false)
            {
                db.MOes.Remove(mO);
            }
            else
            {
                mO.Status = "D";
                db.Entry(mO).Property(x => x.Status).IsModified = true;

            }
            await db.SaveChangesAsync();
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
