using System;
using System.Collections.Generic;
using System.Text;

namespace Moresca_Actions.Products
{
    public class Accrual
    {
        public int accountNumber { get; set; }
        public string accountType { get; set; }
        public double balance { get; set; }
        public bool blockDirectEntries { get; set; }
        public string debitCredit { get; set; }
        public string name { get; set; }
        public string accountingYears { get; set; }
        public string self { get; set; }
    }

    public class ProductGroup
    {
        public int productGroupNumber { get; set; }
        public string name { get; set; }
        public string salesAccounts { get; set; }
        public string products { get; set; }
        public Accrual accrual { get; set; }
        public string self { get; set; }
    }

    public class Unit
    {
        public int unitNumber { get; set; }
        public string name { get; set; }
        public string products { get; set; }
        public string self { get; set; }
    }

    public class Invoices
    {
        public string drafts { get; set; }
        public string booked { get; set; }
        public string self { get; set; }
    }

    public class Pricing
    {
        public string currencySpecificSalesPrices { get; set; }
    }

    public class Collection
    {
        public string productNumber { get; set; }
        public string name { get; set; }
        public double costPrice { get; set; }
        public double recommendedPrice { get; set; }
        public double salesPrice { get; set; }
        public bool barred { get; set; }
        public DateTime lastUpdated { get; set; }
        public ProductGroup productGroup { get; set; }
        public Unit unit { get; set; }
        public Invoices invoices { get; set; }
        public Pricing pricing { get; set; }
        public string self { get; set; }
        public string barCode { get; set; }
        public double qty { get; set; }
    }

    public class Pagination
    {
        public int maxPageSizeAllowed { get; set; }
        public int skipPages { get; set; }
        public int pageSize { get; set; }
        public int results { get; set; }
        public int resultsWithoutFilter { get; set; }
        public string firstPage { get; set; }
        public string nextPage { get; set; }
        public string lastPage { get; set; }
    }

    public class Create
    {
        public string description { get; set; }
        public string href { get; set; }
        public string httpMethod { get; set; }
    }

    public class MetaData
    {
        public Create create { get; set; }
    }

    public class ProductEntities
    {
        public IList<Collection> collection { get; set; }
        public Pagination pagination { get; set; }
        public MetaData metaData { get; set; }
        public string self { get; set; }
    }

    public class Output {
        public string ProductNumber { get; set; }
        public string Name { get; set; }
        public double CostPrice { get; set; }
        public double RecommendedPrice { get; set; }
        public double SalesPrice { get; set; }
        public bool Barred { get; set; }
        public string BarCode { get; set; }
    }
}
