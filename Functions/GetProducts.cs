using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using Moresca_Actions.Products;

namespace Moresca_Actions.Functions
{
    public static class GetProducts
    {
        [FunctionName("GetProducts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to [GetProducts].");

            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            string secretToken = config["X-AppSecretToken"];
            string grantToken = config["X-AgreementGrantToken"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://restapi.e-conomic.com/products/?pagesize=1000");
                client.DefaultRequestHeaders.Add("X-AppSecretToken", secretToken);
                client.DefaultRequestHeaders.Add("X-AgreementGrantToken", grantToken);
                var erpResult = await client.GetAsync("");

                ContentResult result = new ContentResult();
                result.Content = await erpResult.Content.ReadAsStringAsync();

                ProductEntities data = JsonConvert.DeserializeObject<ProductEntities>(result.Content);
                //List<Output> resData = new List<Output>();
                //foreach (Collection coll in data.collection)
                //{
                //    Output output = new Output()
                //    {
                //        BarCode = coll.barCode,
                //        Barred = coll.barred,
                //        CostPrice = coll.costPrice,
                //        Name = coll.name,
                //        ProductNumber = coll.productNumber,
                //        RecommendedPrice = coll.recommendedPrice,
                //        SalesPrice = coll.salesPrice
                //    };

                //    resData.Add(output);
                //}

                JsonResult jr = new JsonResult(data.collection.OrderBy(x => x.name));
                return jr;
            }
        }
    }
}
