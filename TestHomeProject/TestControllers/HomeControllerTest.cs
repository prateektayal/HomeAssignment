using HomeProject.Controllers;
using HomeProject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace TestHomeProject.TestControllers
{
    public class HomeControllerTest
    {
        #region Property  
        public Mock<IConfiguration> mock = new Mock<IConfiguration>();
        #endregion

        #region ==================== Test Methods for GetCarrier1Quote =======================

        /// <summary>
        /// Method to check for null object and Bad request assert
        /// </summary>
        [Fact]
        private void Test_GetCartonsCount_Null()
        {
            //Arrange
            HomeController homeController = new HomeController(mock.Object);

            // Act
            var result = homeController.GetCarrier1Quote(null);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test invalid model object
        /// </summary>
        [Fact]
        private void Test_GetCartonsCount_InValid()
        {
            //Arrange
            HomeController homeController = new HomeController(mock.Object);

            var payload = new WarehouseModel();

            // Act
            var result = homeController.GetCarrier1Quote(payload);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test valid data and ok result
        /// </summary>
        [Fact]
        private async Task Test_GetCartonsCount_Valid()
        {
            //Arrange
            HomeController homeController = new HomeController(mock.Object);

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

            // Act
            var okResult = (await homeController.GetCarrier1Quote(payload)) as OkObjectResult;
            var objectResult = (await homeController.GetCarrier1Quote(payload)) as ObjectResult;
            var jObjResult = JObject.Parse(objectResult.Value.ToString());

            // Assert
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(3000, jObjResult["total"]);
        }

        #endregion

        #region ==================== Test Methods for GetCarrier2Quote =======================

        /// <summary>
        /// Method to check for null object and Bad request assert
        /// </summary>
        [Fact]
        private void Test_GetCartonAmount_Null()
        {
            //Arrange
            HomeController homeController = new HomeController(mock.Object);

            // Act
            var result = homeController.GetCarrier2Quote(null);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test invalid model object
        /// </summary>
        [Fact]
        private void Test_GetCartonAmount_InValid()
        {
            //Arrange
            HomeController homeController = new HomeController(mock.Object);
            var payload = new ConsignmentModel();

            // Act
            var result = homeController.GetCarrier2Quote(payload);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test valid data and ok result
        /// </summary>
        [Fact]
        private async Task Test_GetCartonAmount_Valid()
        {
            //Arrange
            HomeController homeController = new HomeController(mock.Object);

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


            AddressModel address = new AddressModel()
            {
                Address = "Test Address",
                City = "Test City",
                State = "Test State",
                ZipCode = "6a7f88",
                ContactNumber = "1111111"
            };

            var payload = new ConsignmentModel()
            {
                Consignee = address,
                Consignor = address,
                Cartons = cartons
            };

            // Act
            var okResult = (await homeController.GetCarrier2Quote(payload)) as OkObjectResult;
            var objectResult = (await homeController.GetCarrier2Quote(payload)) as ObjectResult;
            var jObjResult = JObject.Parse(objectResult.Value.ToString());

            // Assert
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(11000, jObjResult["amount"]);
        }
        #endregion

        #region ==================== Test Methods for GetCarrier3Quote =======================

        /// <summary>
        /// Method to check for null object and Bad request assert
        /// </summary>
        [Fact]
        private void Test_GetQuote_Null()
        {
            //Arrange
            HomeController homeController = new HomeController(mock.Object);

            // Act
            var result = homeController.GetCarrier3Quote(null);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test invalid model object
        /// </summary>
        [Fact]
        private void Test_GetQuote_InValid()
        {
            //Arrange
            HomeController homeController = new HomeController(mock.Object);
            var payload = new QuoteModel();

            // Act
            var result = homeController.GetCarrier3Quote(payload);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test valid data and ok result
        /// </summary>
        [Fact]
        private async Task Test_GetQuote_Valid()
        {
            //Arrange
            HomeController homeController = new HomeController(mock.Object);

            List<DimensionModel> packages = new List<DimensionModel>();
            packages.Add(new DimensionModel()
            {
                Height = 20,
                Width = 20,
                NoOfCartons = 10
            });

            packages.Add(new DimensionModel()
            {
                Height = 10,
                Width = 10,
                NoOfCartons = 20
            });

            AddressModel address = new AddressModel()
            {
                Address = "Test Address",
                City = "Test City",
                State = "Test State",
                ZipCode = "6a7f88",
                ContactNumber = "1111111"
            };

            var payload = new QuoteModel()
            {
                Source = address,
                Destination = address,
                Packages = packages
            };

            // Act
            var okResult = (await homeController.GetCarrier3Quote(payload)) as OkObjectResult;
            var objectResult = (await homeController.GetCarrier3Quote(payload)) as ObjectResult;
            var model = objectResult.Value as XML;
            // Assert
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(6000.0, model.Quote);
        }
        #endregion

        #region ==================== Test Methods for GetBestQuote ===========================

        /// <summary>
        /// Method to test GetBestQuote ok result
        /// </summary>
        [Fact]
        private async Task Test_GetBestQuote_OkStatus()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            //Arrange
            HomeController homeController = new HomeController(config);

            // Act
            var okResult = (await homeController.GetBestQuote()) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(okResult);

            // Act
            //var task = Task.Run(async () =>
            // {
            //     // Actual test code here.
            //     var objectResult = (await homeController.GetBestQuote()) as ObjectResult;
            //      list = objectResult.Value as List<CarrierQuoteModel>;
            // });


            //var objectResult = await homeController.GetBestQuote();
            //objectResult.Wai
            //list = objectResult.Value as List<CarrierQuoteModel>;

            //task.Wait();
            //task.GetAwaiter().GetResult();


            //Assert.Equal(6000.0, 3);
        }

        #endregion

        #region ==================== Test Methods for AddressModel Model state ===============

        /// <summary>
        /// Method to test invalid model state of AddressModel object
        /// </summary>
        [Fact]
        private void Test_AddressModelInValidModelState()
        {
            AddressModel address = new AddressModel()
            {
                Address = "",
                City = "",
                State = "Test State",
                ZipCode = "6a7f88",
                ContactNumber = "1111111"
            };

            // Act
            var result = CheckValidation(address).Count;

            // Assert
            Assert.NotEqual(0, result);
        }

        /// <summary>
        /// Method to test valid model state of AddressModel object
        /// </summary>
        [Fact]
        private void Test_AddressModelValidModelState()
        {

            AddressModel address = new AddressModel()
            {
                Address = "Test Address",
                City = "Test City",
                State = "Test State",
                ZipCode = "6a7f88",
                ContactNumber = "1111111"
            };

            // Act
            var result = CheckValidation(address).Count;

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        /// <summary>
        /// Method to check the validation state of the model passed to it
        /// </summary>
        /// <param name="model">Mode object to check</param>
        /// <returns>List of Validation result</returns>
        private IList<ValidationResult> CheckValidation(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, result);
            if (model is IValidatableObject) (model as IValidatableObject).Validate(validationContext);

            return result;
        }
    }
}
