using MYBUSINESS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.CustomClasses
{
    public class DAL
    {
        private BusinessContext db = new BusinessContext();

        //public static List<Customer> dbCustomers()
        //{
        //    return db.Customers.Where(x => x.Status != "D").ToList<Customer>();
        //}

        //private List<Supplier> supplier;
        public IQueryable<Supplier> dbSuppliers
        {
            get { return db.Suppliers.Where(x => x.Status != "D"); }
            //set { customers = value; }
        }

        private IQueryable<Customer> customers;
        public IQueryable<Customer> dbCustomers
        {
            get{return db.Customers.Where(x => x.Status != "D");}
            set { customers = value; }
        }

        //private List<Product> products;
        public IQueryable<Product> dbProducts
        {
            get { return db.Products.Where(x => x.Status != "D"); }
            //set { products = value; }
        }
        //public IQueryable<MO> dbMOes
        //{
        //    get { return db.MOes.Where(x => x.Status != "D"); }
        //    //set { products = value; }
        //}


    }
}