using System;
using System.Collections.Generic;
using System.Text;

namespace Moresca_Actions
{
    public class DraftInvoiceRequest
    {
        public string UserEmail { get; set; }
        public Moresca_Actions.Customers.Collection Customer { get; set; } 
        public Moresca_Actions.Products.Collection[] Products { get; set; }
    }

    //public class InvoiceLine {
    //    public string ProductNumber { get; set; }
    //    public double SalesPrice { get; set; }
    //    public double Quantity { get; set; }
    //}
}
