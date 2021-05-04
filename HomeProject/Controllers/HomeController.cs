using HomeProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HomeProject.Controllers
{
    [ApiController]
    [Route("api/home")]
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        readonly HttpClient httpClient = new HttpClient();

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region =================== Carrier POST APIs ========================================

        /// <summary>
        /// Method to get the quote from carrier 1
        /// </summary>
        /// <param name="warehouseModel">WarehouseModel object</param>
        /// <returns>Action Result with total as json object</returns>
        [HttpPost]
        [Route("getcarrier1quote")]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCarrier1Quote([FromBody] WarehouseModel warehouseModel)
        {
            float total = 0;
            if (warehouseModel != null && warehouseModel.PackageDimensions != null)
            {
                if (ModelState.IsValid)
                {
                    // Loop to calculate the amount on the basis of height, width and number of cartons requested 
                    foreach (var dimension in warehouseModel.PackageDimensions.Select((value, index) => new { index, value }))
                    {
                        total = total + (dimension.value.NoOfCartons * dimension.value.Height * dimension.value.Width);
                    }

                    //Creating json object that will retrun total with property name as "total"
                    JObject jObject = new JObject(new JProperty("total", total));
                    return Ok(JsonConvert.SerializeObject(jObject));
                }
                return Conflict(ModelState); // return 409 conflict error 
            }
            return BadRequest("Please check the input data and try again");
        }

        /// <summary>
        /// Method to get the quote from carrier 2
        /// </summary>
        /// <param name="consignmentModel">ConsignmentModel object</param>
        /// <returns>Action result with amount as json object</returns>
        [HttpPost]
        [Route("getcarrier2quote")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCarrier2Quote([FromBody] ConsignmentModel consignmentModel)
        {
            float amount = 0;
            if (consignmentModel != null && consignmentModel.Cartons != null)
            {
                if (ModelState.IsValid)
                {
                    // Loop to calculate the amount on the basis of height, width and number of cartons requested 
                    foreach (var carton in consignmentModel.Cartons.Select((value, index) => new { index, value }))
                    {
                        amount = amount + (carton.value.NoOfCartons * carton.value.Height * carton.value.Width);
                    }

                    //Creating json object that will retrun amount with property name as "amount"
                    JObject jObject = new JObject(new JProperty("amount", amount));

                    /** Putting the thread on sleep for 2 sec to check if there is a deplay in response then API
                     will not wait and will cancel the request according to the cancel time*/
                    Thread.Sleep(2000);
                    return Ok(JsonConvert.SerializeObject(jObject));
                }
                return Conflict(ModelState);
            }
            return BadRequest("Please check the input data and try again");
        }

        /// <summary>
        /// Method to get the quote from carrier 3 in XML format
        /// </summary>
        /// <param name="quoteModel">QuoteModel in XML format</param>
        /// <returns>Action Result with quote value in xml format</returns>
        [Produces("application/xml")]
        [Consumes("application/xml")]
        [HttpPost]
        [Route("getcarrier3quote")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCarrier3Quote([FromBody] QuoteModel quoteModel)
        {
            float quoteAmount = 0;
            if (quoteModel != null && quoteModel.Packages != null)
            {
                if (ModelState.IsValid)
                {
                    // Loop to calculate the amount on the basis of height, width and number of cartons requested 
                    foreach (var package in quoteModel.Packages.Select((value, index) => new { index, value }))
                    {
                        quoteAmount = quoteAmount + (package.value.NoOfCartons * package.value.Height * package.value.Width);
                    }

                    XML xml = new XML();
                    xml.Quote = quoteAmount;
                    return Ok(xml);
                }
                return Conflict(ModelState);
            }
            return BadRequest("Please check the input data and try again");
        }
        #endregion

        #region =================== Get API to get Best Quote ================================

        /// <summary>
        /// Method to get the best quote among the different carrier quotes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getbestquote")]
        public async Task<IActionResult> GetBestQuote()
        {
            IList<CarrierQuoteModel> carrierQuoteList = new List<CarrierQuoteModel>();

            Console.WriteLine("Application started.");


            IList<string> apiList = new List<string>() {
                "getcarrier1quote",
                "getcarrier2quote",
                "getcarrier3quote" };

            try
            {
                cancelTokenSource.CancelAfter(2000);

                await RunApisAsync(apiList, carrierQuoteList);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("\nTasks cancelled: timed out.\n");
            }

            CarrierBestDealModel carrierBestDealModel = new CarrierBestDealModel();
            carrierBestDealModel.Carriers = carrierQuoteList;
            if(carrierQuoteList.Count > 0)
            carrierBestDealModel.BestDeal = carrierQuoteList.FirstOrDefault(x => x.Amount == carrierQuoteList.Min(y => y.Amount));

            return Ok(carrierBestDealModel);
        }
        #endregion

        #region =================== Private methods to execute APIs asynchronously ===========

        /// <summary>
        /// Method to run all APIs asynchronously 
        /// </summary>
        /// <param name="apiList">list of APIs to call</param>
        /// <param name="carrierQuoteList">CarrierQuoteModel list object to return result</param>
        /// <returns></returns>
        private async Task RunApisAsync(IList<string> apiList, IList<CarrierQuoteModel> carrierQuoteList)
        {
            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task>();
            foreach (var api in apiList)
            {
                tasks.Add(ExecuteUrlAsync(api, carrierQuoteList));
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();
        }

        /// <summary>
        /// Method to call APIs and read response asynchronously
        /// </summary>
        /// <param name="api">string api initial</param>
        /// <param name="carrierQuoteList">CarrierQuoteModel list object</param>
        /// <returns></returns>
        private async Task ExecuteUrlAsync(string api, IList<CarrierQuoteModel> carrierQuoteList)
        {
            switch (api.ToLower())
            {
                case "getcarrier1quote":
                    List<DimensionModel> dimension = new List<DimensionModel>();
                    dimension.Add(new DimensionModel()
                    {
                        Height = 20,
                        Width = 20,
                        NoOfCartons = 5
                    });
                    dimension.Add(new DimensionModel()
                    {
                        Height = 10,
                        Width = 10,
                        NoOfCartons = 10
                    });

                    AddressModel address = new AddressModel()
                    {
                        Address = "Test Address",
                        City = "Test City",
                        State = "Test State",
                        ZipCode = "6a7f88",
                        ContactNumber = "1111111"
                    };

                    var payload = new WarehouseModel()
                    {
                        ContactAddress = address,
                        WarehouseAddress = address,
                        PackageDimensions = dimension
                    };


                    HttpResponseMessage response = await httpClient.PostAsync(_configuration.GetValue<string>("BaseUrl") + api, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"), cancelTokenSource.Token);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jObjResult = JObject.Parse(responseBody);
                    carrierQuoteList.Add(new CarrierQuoteModel()
                    {
                        Name = api,
                        Amount = float.Parse(jObjResult["total"].ToString())
                    });
                    break;

                case "getcarrier2quote":

                    List<DimensionModel> cartons = new List<DimensionModel>();
                    cartons.Add(new DimensionModel()
                    {
                        Height = 30,
                        Width = 30,
                        NoOfCartons = 10
                    });
                    cartons.Add(new DimensionModel()
                    {
                        Height = 10,
                        Width = 10,
                        NoOfCartons = 20
                    });


                    AddressModel addressCarrier2 = new AddressModel()
                    {
                        Address = "Test Address",
                        City = "Test City",
                        State = "Test State",
                        ZipCode = "6a7f88",
                        ContactNumber = "1111111"
                    };

                    var payloadCarrier2 = new ConsignmentModel()
                    {
                        Consignee = addressCarrier2,
                        Consignor = addressCarrier2,
                        Cartons = cartons
                    };

                    HttpResponseMessage responseCarrier2 = await httpClient.PostAsync(_configuration.GetValue<string>("BaseUrl") + api, new StringContent(JsonConvert.SerializeObject(payloadCarrier2), Encoding.UTF8, "application/json"), cancelTokenSource.Token);

                    responseCarrier2.EnsureSuccessStatusCode();
                    string responseBodyCarrier2 = await responseCarrier2.Content.ReadAsStringAsync();
                    var jObjResultCarrier2 = JObject.Parse(responseBodyCarrier2);
                    carrierQuoteList.Add(new CarrierQuoteModel()
                    {
                        Name = api,
                        Amount = float.Parse(jObjResultCarrier2["amount"].ToString())
                    });
                    break;

                case "getcarrier3quote":

                    string xmlPayload = "<QuoteModel xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/HomeProject.Model\">" +
                        "<Destination> <Address>Test Source</Address><City>Test</City><ContactNumber>333333</ContactNumber><State>Test</State><ZipCode>86988h</ZipCode></Destination>" +
                        "<Packages>" +
                        "<DimensionModel><Height>40</Height><NoOfCartons>5</NoOfCartons><Width>40</Width></DimensionModel>" +
                        "<DimensionModel><Height>20</Height><NoOfCartons>10</NoOfCartons><Width>20</Width></DimensionModel>" +
                        "<DimensionModel><Height>10</Height><NoOfCartons>15</NoOfCartons><Width>10</Width></DimensionModel>" +
                        "</Packages>" +
                        "<Source><Address>Test Source</Address><City>Test</City><ContactNumber>333333</ContactNumber><State>Test</State><ZipCode>86988h</ZipCode></Source>" +
                        "</QuoteModel>";

                    HttpResponseMessage responseCarrier3 = await httpClient.PostAsync(_configuration.GetValue<string>("BaseUrl") + api, new StringContent(xmlPayload, Encoding.UTF8, "application/xml"), cancelTokenSource.Token);
                    responseCarrier3.EnsureSuccessStatusCode();
                    string responseBodyCarrier3 = await responseCarrier3.Content.ReadAsStringAsync();
                    XDocument xdoc = new XDocument();
                    xdoc = XDocument.Parse(responseBodyCarrier3);
                    carrierQuoteList.Add(new CarrierQuoteModel()
                    {
                        Name = api,
                        Amount = float.Parse(xdoc.Root.Value)
                    });
                    break;
            }

        }
        #endregion
    }
}
