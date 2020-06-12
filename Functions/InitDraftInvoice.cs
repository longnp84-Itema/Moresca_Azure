using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Text;
using Moresca_Actions.Invoices;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Moresca_Actions.Functions
{
    public static class InitDraftInvoice
    {
        [FunctionName("InitDraftInvoice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to [InitDraftInvoice].");

            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
                string secretToken = config["X-AppSecretToken"];
                string grantToken = config["X-AgreementGrantToken"];

                //int customerNumber = Convert.ToInt32(req.Query["CustomerNumber"]);
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                DraftInvoiceRequest draft = JsonConvert.DeserializeObject<DraftInvoiceRequest>(requestBody);

                // save into database
                string connectionString = Environment.GetEnvironmentVariable("sqldb_connection");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string newOrderId = Guid.NewGuid().ToString();
                    string query = "INSERT INTO dbo.OrderHistory(Id, UserEmail) " +
                            $"Values('{newOrderId}', '{draft.UserEmail}');";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Execute the command and log the # rows affected.
                        var rows = await cmd.ExecuteNonQueryAsync();
                        log.LogInformation($"{rows} rows were added into OrderHistory.");
                    }

                    foreach (var product in draft.Products)
                    {
                        string query2 = "INSERT INTO dbo.OrderDetailHistory(OrderId, ProductNumber, ProductName, SalePrice, Quantity) " +
                            $"Values('{newOrderId}', '{product.productNumber}', '{product.name}', {product.salesPrice}, {product.qty});";

                        using (SqlCommand cmd = new SqlCommand(query2, conn))
                        {
                            // Execute the command and log the # rows affected.
                            var rows = await cmd.ExecuteNonQueryAsync();
                            log.LogInformation($"{rows} rows were added into OrderDetailHistory.");
                        }
                    }
                }

                using (var client = new HttpClient())
                {
                    DraftInvoiceEntity invoice = GenerateInvoice(draft);

                    var stringContent = new StringContent(JsonConvert.SerializeObject(invoice), UnicodeEncoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("X-AppSecretToken", secretToken);
                    client.DefaultRequestHeaders.Add("X-AgreementGrantToken", grantToken);
                    var erpResult = await client.PostAsync(
                        new Uri("https://restapi.e-conomic.com/invoices/drafts"),
                        stringContent);

                    JsonResult res = new JsonResult(erpResult);
                    return res;
                }
            }
            catch (Exception ex) {
                JsonResult res = new JsonResult(ex);
                return res;
            }
        }


        // Mandatory:
        //currency, customer, date, layout, paymentTerms, recipient, recipient.name, recipient.vatZone
        private static DraftInvoiceEntity GenerateInvoice(DraftInvoiceRequest draft)
        {
            DraftInvoiceEntity invoice = new DraftInvoiceEntity();
            invoice.currency = draft.Customer.currency;
            invoice.netAmount = draft.Products.Sum(x => x.salesPrice);
            invoice.netAmountInBaseCurrency = draft.Products.Sum(x => x.salesPrice);
            invoice.grossAmount = 11;
            invoice.roundingAmount = 10;
            invoice.customer = new Customer()
            {
                customerNumber = draft.Customer.customerNumber,
                self = draft.Customer.self
            };
            invoice.date = DateTime.Now.ToString("yyyy-MM-dd");

            // NOT SURE
            invoice.delivery = new Delivery();
            invoice.references = new References();
            invoice.exchangeRate = 100;
            invoice.notes = new Notes() { 
                heading = "This is created from Moresca's Power Application."
            };

            // need default values
            invoice.layout = new Layout()
            {
                layoutNumber = draft.Customer.layouts != null ? draft.Customer.layouts.layoutNumber : 19,
                //self = reqInvoice.Customer.layouts != null ? reqInvoice.Customer.layouts.self: string.Empty
            };

            // need default values
            invoice.paymentTerms = new PaymentTerms()
            {
                paymentTermsNumber = draft.Customer.paymentTerms != null ? draft.Customer.paymentTerms.paymentTermsNumber : 1
            };
            invoice.recipient = new Recipient()
            {
                name = draft.Customer.name,
                //address = reqInvoice.Customer.address,
                //zip = reqInvoice.Customer.zip,
                //city = reqInvoice.Customer.city,

                // need default values
                vatZone = new VatZone()
                {
                    vatZoneNumber = draft.Customer.vatZone.vatZoneNumber,
                    self = draft.Customer.vatZone.self
                }
                //vatZone = new VatZone()
                //{
                //    name = "Domestic",
                //    vatZoneNumber = 1,
                //    enabledForCustomer = true,
                //    enabledForSupplier = true
                //}
            };
            invoice.lines = new List<Line>() { };
            for (int i = 0; i < draft.Products.Length; i++)
            {
                Moresca_Actions.Products.Collection product = draft.Products[i];

                invoice.lines.Add(new Line()
                {
                    lineNumber = i + 1,
                    description = product.name,
                    product = new Product()
                    {
                        productNumber = product.productNumber,
                        self = product.self
                    },
                    quantity = product.qty,
                    unitNetPrice = product.salesPrice,
                    unit = new Unit()
                    {
                        unitNumber = product.unit.unitNumber,
                        name = product.unit.name,
                        self = product.unit.self
                    }
                });
            }

            return invoice;
        }
    }
}
