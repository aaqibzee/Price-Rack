using Microsoft.Extensions.Logging;
using Moq;
using PriceMicroservice.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using PriceRack.DataAccess.Entities;
using PriceRack.Services;
using PriceRack.Common;
using Mapster;
using PriceMicroservice.DAL;

namespace PriceRack.Tests.ControllersTests
{
    [TestClass]
    public class ServciesTests
    {
        
        private Mock<IPriceAggregationService> _priceAggregationServiceMock;
        private Mock<IBitFinexApiService> _bitsFinexApiServiceMock;
        private Mock<IBitsMapApiService> _bitsMapApiServiceMock;
        private Mock<ILogger<PriceService>> _loggerMock;
        private Mock<IPriceDAL> _priceDALMock;
        private IPriceService _priceService;

        [TestInitialize()]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger<PriceService>>();

            _priceDALMock = new Mock<IPriceDAL>();
            _bitsMapApiServiceMock = new Mock<IBitsMapApiService>();
            _priceAggregationServiceMock = new Mock<IPriceAggregationService>();
            _bitsFinexApiServiceMock = new Mock<IBitFinexApiService>();
            _priceService = new PriceService(_loggerMock.Object, _priceDALMock.Object, _priceAggregationServiceMock.Object,
                _bitsMapApiServiceMock.Object, _bitsFinexApiServiceMock.Object);
        }

        [TestMethod]
        public async Task GetAggregatedPrice_ReturnsPriceModel()
        {
            // Arrange
            var time = new DateTime(2022, 01, 01, 10, 0, 0, DateTimeKind.Utc);
            var expectedPriceModel = new Price
            {
                Time = time,
                Instrument = Constants.BTCInstrumentName,
                Value = 55000.00m
            };

            _priceDALMock.Setup(x => x.GetAggregatedPriceAsync(time).Result)
                .Returns(expectedPriceModel);

            // Act
            var result = await _priceService.GetAggregatedPrice(time);

            // Assert
            Assert.AreEqual(expectedPriceModel.Time, result.Time);
            Assert.AreEqual(expectedPriceModel.Instrument, result.Instrument);
            Assert.AreEqual(expectedPriceModel.Value, result.Value);
        }
        [TestMethod]
        public async Task GetAggregatedPrice_ReturnsPriceModel_FromService()
        {
            // Arrange
            var time = new DateTime(2022, 01, 01, 10, 0, 0, DateTimeKind.Utc);
            var expectedPrice = 55000.00m;

            _priceDALMock.Setup(x => x.GetAggregatedPriceAsync(time).Result)
                .Returns<PriceModel>(null);

            _priceAggregationServiceMock.Setup(x => x.GetAggregatedPrice(It.IsAny<IEnumerable<Decimal>>()))
                .Returns(expectedPrice);

            // Act
            var result = await _priceService.GetAggregatedPrice(time);

            // Assert
            Assert.AreEqual(time, result.Time);
            Assert.AreEqual(Constants.BTCInstrumentName, result.Instrument);
            Assert.AreEqual(expectedPrice, result.Value);
        }
        [TestMethod]
        public async Task GetAggregatedPrice_ReturnsPriceFromDatabase_WhenAvailable()
        {
            // Arrange
            var time = new DateTime(2022, 01, 01, 10, 0, 0, DateTimeKind.Utc);
            var priceModel = new PriceModel
            {
                Time = time,
                Instrument = "BTC",
                Value = 50000.00m
            };
            var priceEntity = priceModel.Adapt<Price>();
            _priceDALMock.Setup(x => x.GetAggregatedPriceAsync(time)).ReturnsAsync(priceEntity);

            // Act
            var result = await _priceService.GetAggregatedPrice(time);

            // Assert
            Assert.AreEqual(priceModel.Instrument, result.Instrument);
            Assert.AreEqual(priceModel.Value, result.Value);
            Assert.AreEqual(priceModel.Time, result.Time);
            _priceDALMock.Verify(x => x.GetAggregatedPriceAsync(time), Times.Once);
        }
        [TestMethod]
        public async Task GetAggregatedPrice_CallsGetPrice_AndReturnsAggregatedPrice_WhenNotAvailableInDatabase()
        {
            // Arrange
            var time = new DateTime(2022, 01, 01, 10, 0, 0, DateTimeKind.Utc);
            var price = 50000.00m;
            _priceDALMock.Setup(x => x.GetAggregatedPriceAsync(time)).ReturnsAsync((Price)null);
            _priceAggregationServiceMock.Setup(x => x.GetAggregatedPrice(It.IsAny<IEnumerable<Decimal>>()))
                .Returns(price);

            // Act
            var result = await _priceService.GetAggregatedPrice(time);

            // Assert
            Assert.AreEqual(price, result.Value);
            Assert.AreEqual(Constants.BTCInstrumentName, result.Instrument);
            Assert.AreEqual(time, result.Time);
        }
        [TestMethod]
        public async Task GetAggregatedPrice_ReturnsPriceFromDatabase()
        {
            // Arrange
            var time = DateTime.UtcNow;
            var price = new Price { Time = time, Instrument = "BTC", Value = 50000.0M };
            var priceDALMock = new Mock<IPriceDAL>();
            priceDALMock.Setup(x => x.GetAggregatedPriceAsync(time).Result).Returns(price);

            var priceService = new PriceService(Mock.Of<ILogger<PriceService>>(), priceDALMock.Object,
                Mock.Of<IPriceAggregationService>(), Mock.Of<IBitsMapApiService>(), Mock.Of<IBitFinexApiService>());

            // Act
            var result = await priceService.GetAggregatedPrice(time);

            // Assert
            Assert.AreEqual(price.Time, result.Time);
            Assert.AreEqual(price.Instrument, result.Instrument);
            Assert.AreEqual(price.Value, result.Value);
            priceDALMock.Verify(x => x.GetAggregatedPriceAsync(time), Times.Once);
        }
        [TestMethod]
        public async Task GetAggregatedPrice_ReturnsPriceFromExternalAPI()
        {
            // Arrange
            var time = DateTime.UtcNow;
            var priceDALMock = new Mock<IPriceDAL>();
            priceDALMock.Setup(x => x.GetAggregatedPriceAsync(time).Result).Returns((Price)null);

            var priceAggregationServiceMock = new Mock<IPriceAggregationService>();
            priceAggregationServiceMock.Setup(x => x.GetAggregatedPrice(It.IsAny<IEnumerable<decimal>>())).Returns(50000.00M);

            var priceService = new PriceService(Mock.Of<ILogger<PriceService>>(), priceDALMock.Object,
                priceAggregationServiceMock.Object, Mock.Of<IBitsMapApiService>(), Mock.Of<IBitFinexApiService>());

            // Act
            var result = await priceService.GetAggregatedPrice(time);

            // Assert
            Assert.AreEqual(time, result.Time);
            Assert.AreEqual(Constants.BTCInstrumentName, result.Instrument);
            Assert.AreEqual(50000.00M, result.Value);
        }
        [TestMethod]
        public async Task GetAggregatedPrice_ThrowsException()
        {
            // Arrange
            var time = DateTime.UtcNow;
            _priceDALMock.Setup(x => x.GetAggregatedPriceAsync(It.IsAny<DateTime>())).ThrowsAsync(new Exception("Unable to fetch price from database."));
            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _priceService.GetAggregatedPrice(time));
        }
    }
}