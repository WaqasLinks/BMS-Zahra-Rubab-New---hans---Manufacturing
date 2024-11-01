using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ProductViewModel
    {
        //public IQueryable<Customer> Customers { get; set; }
        //public Customer Customer { get; set; }
        //public SO SaleOrder { get; set; }
        public List<SubItem> SubItems { get; set; }
        public IQueryable<Product> Products { get; set; }
        public Product Product { get; set; }
    }
}