using HomeProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace HomeProject.Controllers
{
    [ApiController]
    [Route("api/home")]
    public class HomeController : Controller
    {

        /// <summary>
        /// Method to Get the total for package
        /// </summary>
        /// <param name="warehouseModel">WarehouseModel object</param>
        /// <returns>Action Result with total as json object</returns>
        [HttpPost]
        [Route("getcartonscount")]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartonsCount([FromBody] WarehouseModel warehouseModel)
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
                return Conflict(ModelState);
            }
            return BadRequest("Please check the input data and try again");
        }

        /// <summary>
        /// Method to get the amount for cartons
        /// </summary>
        /// <param name="consignmentModel">ConsignmentModel object</param>
        /// <returns>Action result with amount as json object</returns>
        [HttpPost]
        [Route("getcartonamount")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartonAmount([FromBody] ConsignmentModel consignmentModel)
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
                    return Ok(JsonConvert.SerializeObject(jObject));
                }
                return Conflict(ModelState);
            }
            return BadRequest("Please check the input data and try again");
        }

        /// <summary>
        /// Method to get best quote in XML format
        /// </summary>
        /// <param name="quoteModel">QuoteModel in XML format</param>
        /// <returns>Action Result with quote value in xml format</returns>
        [Produces("application/xml")]
        [Consumes("application/xml")]
        [HttpPost]
        [Route("getquote")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuote([FromBody] QuoteModel quoteModel)
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
    }
}
