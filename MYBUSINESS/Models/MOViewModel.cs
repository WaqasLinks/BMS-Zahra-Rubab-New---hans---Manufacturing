using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class MOViewModel
    {
        //public IQueryable<Customer> Customers { get; set; }
        //public Customer Customer { get; set; }
        //public SO SaleOrder { get; set; }
        public List<Component> Components { get; set; }
        public IQueryable<MO> MOes { get; set; }
        public MO MO { get; set; }
    }
}