using HomeProject.Controllers;
using HomeProject.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace TestHomeProject.TestControllers
{
    public class HomeControllerTest
    {
        //Arrange
        readonly HomeController homeController = new HomeController();

        #region ==================== Test Methods for GetCartonsCount ==========================

        /// <summary>
        /// Method to check for null object and Bad request assert
        /// </summary>
        [Fact]
        public void Test_GetCartonsCount_Null()
        {
            // Act
            var result = homeController.GetCartonsCount(null);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test invalid model object
        /// </summary>
        [Fact]
        public void Test_GetCartonsCount_InValid()
        {
            var payload = new WarehouseModel();

            // Act
            var result = homeController.GetCartonsCount(payload);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test valid data and ok result
        /// </summary>
        [Fact]
        public async Task Test_GetCartonsCount_Valid()
        {
            List<Dimension> dimension = new List<Dimension>();
            dimension.Add(new Dimension()
            {
                Height = 20,
                Width = 20,
                NoOfCartons = 5
            });
            dimension.Add(new Dimension()
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
            var okResult = (await homeController.GetCartonsCount(payload)) as OkObjectResult;
            var objectResult = (await homeController.GetCartonsCount(payload)) as ObjectResult;
            var jObjResult = JObject.Parse(objectResult.Value.ToString());

            // Assert
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(3000, jObjResult["total"]);
        }       

        #endregion

        #region ==================== Test Methods for GetCartonAmount ==========================

        /// <summary>
        /// Method to check for null object and Bad request assert
        /// </summary>
        [Fact]
        public void Test_GetCartonAmount_Null()
        {
            // Act
            var result = homeController.GetCartonAmount(null);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test invalid model object
        /// </summary>
        [Fact]
        public void Test_GetCartonAmount_InValid()
        {
            var payload = new ConsignmentModel();

            // Act
            var result = homeController.GetCartonAmount(payload);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test valid data and ok result
        /// </summary>
        [Fact]
        public async Task Test_GetCartonAmount_Valid()
        {
            List<Dimension> cartons = new List<Dimension>();
            cartons.Add(new Dimension()
            {
                Height = 30,
                Width = 30,
                NoOfCartons = 10
            });
            cartons.Add(new Dimension()
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
            var okResult = (await homeController.GetCartonAmount(payload)) as OkObjectResult;
            var objectResult = (await homeController.GetCartonAmount(payload)) as ObjectResult;
            var jObjResult = JObject.Parse(objectResult.Value.ToString());

            // Assert
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(11000, jObjResult["amount"]);
        }
        #endregion

        #region ==================== Test Methods for GetQuote =================================

        /// <summary>
        /// Method to check for null object and Bad request assert
        /// </summary>
        [Fact]
        public void Test_GetQuote_Null()
        {
            // Act
            var result = homeController.GetQuote(null);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test invalid model object
        /// </summary>
        [Fact]
        public void Test_GetQuote_InValid()
        {
            var payload = new QuoteModel();

            // Act
            var result = homeController.GetQuote(payload);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Method to test valid data and ok result
        /// </summary>
        [Fact]
        public async Task Test_GetQuote_Valid()
        {
            List<Dimension> packages = new List<Dimension>();
            packages.Add(new Dimension()
            {
                Height = 20,
                Width = 20,
                NoOfCartons = 10
            });

            packages.Add(new Dimension()
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
            var okResult = (await homeController.GetQuote(payload)) as OkObjectResult;
            var objectResult = (await homeController.GetQuote(payload)) as ObjectResult;
            var model = objectResult.Value as XML;
            // Assert
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(6000.0, model.Quote);


        }
        #endregion

        #region ==================== Test Methods for AddressModel Model state =================

        /// <summary>
        /// Method to test invalid model state of AddressModel object
        /// </summary>
        [Fact]
        public void Test_AddressModelInValidModelState()
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
        public void Test_AddressModelValidModelState()
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
        public IList<ValidationResult> CheckValidation(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, result);
            if (model is IValidatableObject) (model as IValidatableObject).Validate(validationContext);

            return result;
        }
    }
}
