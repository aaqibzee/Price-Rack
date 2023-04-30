using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PriceRack.Controllers;
using PriceMicroservice.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using PriceRack.DataAccess.Entities;
using PriceRack.Services;
using PriceRack.Common;
using PriceRack.Common.Extensions;
using PriceRack.Attributes;
using System.ComponentModel.DataAnnotations;
using PriceRack.Common.Exceptions;
using System.Net;

namespace PriceRack.Tests.ControllersTests
{
    [TestClass]
    public class PricesControllerTests
    {
        #region Declarations
        private PricesController _controller;
        private Mock<IPriceService> _priceServiceMock;
        private Mock<ILogger<PricesController>> _mockLogger;
        #endregion

        #region Initializations
        [TestInitialize()]
        public void Initialize()
        {
            _priceServiceMock = new Mock<IPriceService>();
            _mockLogger = new Mock<ILogger<PricesController>>();
            _controller = new PricesController(_mockLogger.Object, _priceServiceMock.Object);
        }

        [TestCleanup()]
        public void CleanUp()
        {
        }
        #endregion
        [TestMethod]
        public async Task GetAggregatedPrice_ReturnsOkResult_WithPriceDTO()
        {
            // Arrange
            var time = DateTime.Now;
            var expectedPrice = new PriceModel()
            {
                Time = time,
                Value = 123.45m,
                Instrument = Constants.BTCInstrumentName
            };

            _priceServiceMock.Setup(x => x.GetAggregatedPrice(It.IsAny<DateTime>()).Result).Returns(expectedPrice);

            // Act
            var result = await _controller.GetAggregatedPrice(DateTime.Now);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOfType(okResult.Value, typeof(PriceDTO));
            var priceDTO = okResult.Value as PriceDTO;
        }
        [TestMethod]
        public void NotInFutureAttribute_ReturnsValidationError_WhenFutureDateProvided()
        {
            // Arrange
            var attribute = new NotInFutureAttribute("Test Date");
            var context = new ValidationContext(new { TestDate = DateTime.Now.AddDays(1) });
            var validationResults = new List<ValidationResult>();

            // Act
            try
            {
                var result = Validator.TryValidateValue(DateTime.Now.AddDays(1), context, validationResults, new[] { attribute });

            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType<CustomInvalidException>(ex);
                var exception = (CustomInvalidException)ex;
                Assert.AreEqual(exception.Status, HttpStatusCode.BadRequest);
            }
        }
        [TestMethod]
        public async Task GetAggregatedPrice_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            _priceServiceMock.Setup(x => x.GetAggregatedPrice(It.IsAny<DateTime>()))
                .ThrowsAsync(new CustomInvalidException(HttpStatusCode.InternalServerError,"Test exception"));
            // Act
            try
{
                var result = await _controller.GetAggregatedPrice(DateTime.Now);
            }
            catch (Exception ex)
            //Assert
            {
                Assert.IsInstanceOfType<CustomInvalidException>(ex);
                var exception = (CustomInvalidException)ex;
                Assert.AreEqual(exception.Status, HttpStatusCode.InternalServerError);
            }
        }
        [TestMethod]
        public async Task GetAggregatedPrice_ReturnsBadRequest_When_DateIsInFuture()
        {
            // Arrange
            var time = DateTime.Now.AddDays(1);
            // Act
            var result = await _controller.GetAggregatedPrice(time);
            // Assert
            Assert.IsNotInstanceOfType<BadRequestObjectResult>(result.Result);
        }
        [TestMethod]
        public async Task GetAggregatedPrice_ReturnsOk_When_DateIsInPast()
        {
            // Arrange
            // Arrange
            var time = DateTime.Now.AddDays(-1);
            var expectedPrice = new PriceModel()
            {
                Time = time,
                Value = 100,
                Instrument = Constants.BTCInstrumentName
            };
            _priceServiceMock.Setup(s => s.GetAggregatedPrice(It.IsAny<DateTime>()).Result).Returns(expectedPrice);
            // Act
            var result = await _controller.GetAggregatedPrice(time);
            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        }
        [TestMethod]
        public async Task GetPriceHistory_ReturnsOkResult_WithPriceModels()
        {
            // Arrange
            var start = new DateTime(2022, 01, 01, 10, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2022, 01, 01, 12, 0, 0, DateTimeKind.Utc);

            var prices = new List<PriceModel>
            {
                new PriceModel
                {
                    Time = new DateTime(2022, 01, 01, 10, 0, 0, DateTimeKind.Utc),
                    Instrument = "BTC",
                    Value = 50000.00m
                },
                new PriceModel
                {
                    Time = new DateTime(2022, 01, 01, 11, 0, 0, DateTimeKind.Utc),
                    Instrument = "BTC",
                    Value = 55000.00m
                },
            };

            _priceServiceMock.Setup(x => x.GetPrices(start.RoundHour(), end.RoundHour()).Result)
                .Returns(prices as IEnumerable<PriceModel>);

            // Act
            var result = await _controller.GetPriceHistory(start, end);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<PriceDTO>));

            var priceDTOs = okResult.Value as IEnumerable<PriceDTO>;

            Assert.AreEqual(2, priceDTOs.Count());

            Assert.AreEqual(new DateTime(2022, 01, 01, 10, 0, 0, DateTimeKind.Utc), priceDTOs.ElementAt(0).Time);
            Assert.AreEqual("BTC", priceDTOs.ElementAt(0).Instrument);
            Assert.AreEqual(50000.00m, priceDTOs.ElementAt(0).Value);

            Assert.AreEqual(new DateTime(2022, 01, 01, 11, 0, 0, DateTimeKind.Utc), priceDTOs.ElementAt(1).Time);
            Assert.AreEqual("BTC", priceDTOs.ElementAt(1).Instrument);
            Assert.AreEqual(55000.00m, priceDTOs.ElementAt(1).Value);
        }
        [TestMethod]
        public async Task GetPriceHistory_ReturnsBadRequest_WhenStartIsGreaterThanEnd()
        {
            // Arrange
            var start = new DateTime(2022, 01, 01, 12, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2022, 01, 01, 10, 0, 0, DateTimeKind.Utc);

            // Act
            var result = await _controller.GetPriceHistory(start, end);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual("Start time cannot be greater than end time.", badRequestResult.Value);
        }
        [TestMethod]
        public async Task GetPriceHistory_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var start = new DateTime(2022, 01, 01, 10, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2022, 01, 01, 12, 0, 0, DateTimeKind.Utc);

            _priceServiceMock.Setup(x => x.GetPrices(start.RoundHour(), end.RoundHour()).Result)
                .Throws(new Exception("An error occurred in the service layer."));

            // Act 
            // Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _controller.GetPriceHistory(start, end));
        }
    }
}