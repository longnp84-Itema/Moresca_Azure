using System;
using System.Collections.Generic;
using System.Text;

namespace Moresca_Actions.Invoices
{
    public class PaymentTerms
    {
        public int paymentTermsNumber { get; set; }
        //public int daysOfCredit { get; set; }
        //public string name { get; set; }
        //public string paymentTermsType { get; set; }
    }

    public class Customer
    {
        public int customerNumber { get; set; }
        public string self { get; set; }
    }

    public class VatZone
    {
        public string self { get; set; }
        public int vatZoneNumber { get; set; }
    }

    public class Recipient
    {
        public string name { get; set; }
        //public string address { get; set; }
        //public string zip { get; set; }
        //public string city { get; set; }
        public VatZone vatZone { get; set; }
    }

    public class Delivery
    {
        //public string address { get; set; }
        //public string zip { get; set; }
        //public string city { get; set; }
        //public string country { get; set; }
        //public string deliveryDate { get; set; }
    }

    public class References
    {
        //public string other { get; set; }
    }

    public class Notes {
        public string heading { get; set; }
        //public string textLine1 { get; set; }
        //public string textLine2 { get; set; }
    }
    public class Layout
    {
        public int layoutNumber { get; set; }
        //public string self { get; set; }
    }

    public class Unit
    {
        public int unitNumber { get; set; }
        public string name { get; set; }
        public string self { get; set; }
    }

    public class Product
    {
        public string productNumber { get; set; }
        public string self { get; set; }
    }

    public class Line
    {
        public int lineNumber { get; set; }
        public string description { get; set; }
        //public int sortKey { get; set; }
        public Unit unit { get; set; }
        public Product product { get; set; }
        public double quantity { get; set; }
        public double unitNetPrice { get; set; }
        public double discountPercentage { get; set; }
        public double unitCostPrice { get; set; }
        public double totalNetAmount { get; set; }
        public double marginInBaseCurrency { get; set; }
        public double marginPercentage { get; set; }
    }

    public class DraftInvoiceEntity
    {
        public string date { get; set; }
        public string currency { get; set; }
        public double exchangeRate { get; set; }
        public double netAmount { get; set; }
        public double netAmountInBaseCurrency { get; set; }
        public double grossAmount { get; set; }
        public double marginInBaseCurrency { get; set; }
        public double marginPercentage { get; set; }
        public double vatAmount { get; set; }
        public double roundingAmount { get; set; }
        public double costPriceInBaseCurrency { get; set; }
        public PaymentTerms paymentTerms { get; set; }

        public Notes notes { get; set; }
        public Customer customer { get; set; }
        public Recipient recipient { get; set; }
        public Delivery delivery { get; set; }
        public References references { get; set; }
        public Layout layout { get; set; }
        public IList<Line> lines { get; set; }
    }


}
