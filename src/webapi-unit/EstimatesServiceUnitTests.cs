using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using BodyShopBoosterTest.Data;
using BodyShopBoosterTest.Enumerations;
using BodyShopBoosterTest.Exceptions;
using BodyShopBoosterTest.Models;
using BodyShopBoosterTest.Services;
using Moq;
using Xunit;

namespace BodyShopBoosterTest_UnitTests
{
    /// <summary>Unit tests for the <see cref="EstimatesService"/> service.</summary>
    public class EstimatesServiceUnitTests
    {
        // INSTANCE METHODS
        /// <summary>Retrieves a new instance of a valid <see cref="Estimate"/> object to be used in the tests.</summary>
        /// <returns>
        ///     Returns the newly instantiated <see cref="Estimate"/> object.
        ///     This object initially contains all-valid data.
        /// </returns>
        private Estimate InstantiateValidEstimateData() =>
            new Estimate
            {
                FirstName = "John",
                LastName = "Doe",
                CarType = VehicleType.Truck,
                LicensePlate = "ABCD-123",
                Model = "SomeModelHere",
                Year = "2013",
                Status = EstimateStatus.Pending,
            };


        /// <summary>Utility method for creating a mocked <see cref="AppDbContext"/> instance.</summary>
        /// <remarks>
        ///     <para>
        ///         The mocked <see cref="AppDbContext"/> instance will be able to simulate calls to some operations
        ///         like <see cref="Microsoft.EntityFrameworkCore.DbContext.AddAsync{TEntity}(TEntity, CancellationToken)"/>
        ///         and <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(CancellationToken)"/>.
        ///     </para>
        ///     <para>This method can also take an <see cref="Action{T}"/> method that can be used to further configure scenario-specific options.</para>
        /// </remarks>
        /// <param name="extraSetups">An optional action to be called to perform extra configuration steps for the mocked object, if necessary.</param>
        /// <returns>Returns the mocked <see cref="AppDbContext"/> instance.</returns>
        private AppDbContext CreateMockAppDbContext(Action<Mock<AppDbContext>> extraSetups = null)
        {
            // Create an AppDbContext and simulate its AddAsync() and SaveChangesAsync() methods
            Estimate addedObject = null;
            var appDbContextMock = new Mock<AppDbContext>();
            appDbContextMock.Setup(context => context.AddAsync(It.IsAny<Estimate>(), It.IsAny<CancellationToken>()))
                .Callback<Estimate, CancellationToken>((addedEstimate, _) => {
                    addedObject = addedEstimate;
                });
            appDbContextMock.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => {
                    // Assigns a new ID to the Estimate, as it happens when DbContext.SaveChangesAsync(...) is called
                    addedObject.Id = Guid.NewGuid();
                });

            // If this method has received any extra setup actions to be executed, then execute them
            extraSetups?.Invoke(appDbContextMock);

            // Return resulting mocked AppDbContext
            return appDbContextMock.Object;
        }





        // TESTS
        [Fact]
        public async Task CreateEstimateAsync_ValidData_ReturnsValidEstimate()
        {
            // Arrange
            var inputData = InstantiateValidEstimateData();
            var inputValidationContext = new ValidationContext(inputData);
            var inputValidationResults = new List<ValidationResult>();

            var mockAppDbContext = CreateMockAppDbContext();
            var estimatesService = new EstimatesService(mockAppDbContext);


            // Act
            bool inputDataHasEmptyId = (inputData.Id == Guid.Empty);
            bool isValidInputData = Validator.TryValidateObject(inputData, inputValidationContext, inputValidationResults);

            var outputData = await estimatesService.CreateEstimateAsync(inputData);
            var outputValidationContext = new ValidationContext(outputData);
            var outputValidationResults = new List<ValidationResult>();
            bool isValidOutputData = Validator.TryValidateObject(outputData, inputValidationContext, inputValidationResults);

            bool outputDataHasInsertionId = (outputData.Id != Guid.Empty);


            // Assert
            Assert.True(inputDataHasEmptyId);
            Assert.True(isValidInputData);
            Assert.True(isValidOutputData);
            Assert.True(outputDataHasInsertionId);
        }


        [Fact]
        public async Task CreateEstimateAsync_DataWithPredefinedId_ThrowsServiceException()
        {
            // Arrange
            var inputData = InstantiateValidEstimateData();
            inputData.Id = Guid.NewGuid();

            var mockAppDbContext = CreateMockAppDbContext();
            var estimatesService = new EstimatesService(mockAppDbContext);


            // Act
            ServiceException caughtException = null;
            try
            {
                await estimatesService.CreateEstimateAsync(inputData);
            }
            catch (ServiceException svcEx)
            {
                caughtException = svcEx;
            }


            // Assert
            Assert.NotNull(caughtException);
            Assert.False(string.IsNullOrWhiteSpace(caughtException.AppErrorCode));
            Assert.NotNull(caughtException.ValidationErrors);
            Assert.NotEmpty(caughtException.ValidationErrors);
            Assert.Contains(caughtException.ValidationErrors.Keys, errorKey => errorKey == nameof(Estimate.Id));
        }


        [Theory]
        [InlineData(EstimateStatus.Sent)]
        [InlineData(EstimateStatus.BookConfirmed)]
        public async Task CreateEstimateAsync_DataWithNonPendingStatus_ThrowsServiceException(EstimateStatus statusToTest)
        {
            // Arrange
            var inputData = InstantiateValidEstimateData();
            inputData.Status = statusToTest;

            var inputValidationContext = new ValidationContext(inputData);
            var inputValidationResults = new List<ValidationResult>();

            var mockAppDbContext = CreateMockAppDbContext();
            var estimatesService = new EstimatesService(mockAppDbContext);


            // Act
            bool isValidInputData = Validator.TryValidateObject(inputData, inputValidationContext, inputValidationResults);
            ServiceException caughtException = null;
            try
            {
                await estimatesService.CreateEstimateAsync(inputData);
            }
            catch (ServiceException svcEx)
            {
                caughtException = svcEx;
            }


            // Assert
            Assert.True(isValidInputData);
            Assert.NotNull(caughtException);
            Assert.False(string.IsNullOrWhiteSpace(caughtException.AppErrorCode));
            Assert.NotNull(caughtException.ValidationErrors);
            Assert.NotEmpty(caughtException.ValidationErrors);
            Assert.Contains(caughtException.ValidationErrors.Keys, errorKey => errorKey == nameof(Estimate.Status));
        }


        [Fact]
        public async Task CreateEstimateAsync_AddAsyncThrowsException_ThrowsServiceException()
        {
            // Arrange
            var inputData = InstantiateValidEstimateData();

            var mockAppDbContext = CreateMockAppDbContext(mockConfigs => {
                mockConfigs.Setup(ctx => ctx.AddAsync(It.IsAny<Estimate>(), It.IsAny<CancellationToken>()))
                    .Throws<Exception>();
            });
            var estimatesService = new EstimatesService(mockAppDbContext);


            // Act
            ServiceException caughtException = null;
            try
            {
                await estimatesService.CreateEstimateAsync(inputData);
            }
            catch (ServiceException svcEx)
            {
                caughtException = svcEx;
            }


            // Assert
            Assert.NotNull(caughtException);
            Assert.Equal(caughtException.AppErrorCode, AppExceptionErrorCodes.DatabaseUpdateError);
        }


        [Fact]
        public async Task CreateEstimateAsync_SaveChangesAsyncThrowsException_ThrowsServiceException()
        {
            // Arrange
            var inputData = InstantiateValidEstimateData();

            var mockAppDbContext = CreateMockAppDbContext(mockConfigs => {
                mockConfigs.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .Throws<Exception>();
            });
            var estimatesService = new EstimatesService(mockAppDbContext);


            // Act
            ServiceException caughtException = null;
            try
            {
                await estimatesService.CreateEstimateAsync(inputData);
            }
            catch (ServiceException svcEx)
            {
                caughtException = svcEx;
            }


            // Assert
            Assert.NotNull(caughtException);
            Assert.Equal(caughtException.AppErrorCode, AppExceptionErrorCodes.DatabaseUpdateError);
        }
    }
}
