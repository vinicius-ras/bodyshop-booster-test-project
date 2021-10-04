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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BodyShopBoosterTest_UnitTests
{
    /// <summary>Unit tests for the <see cref="EstimatesService"/> service.</summary>
    public class EstimatesServiceUnitTests
    {
        // INSTANCE METHODS
        /// <summary>Retrieves a new instance of a valid <see cref="Estimate"/> object to be used in the tests.</summary>
        /// <param name="withValidId">
        ///     A flag indicating if the new instance should be created with a valid ID (a newly generated, random GUID), or with a default/unset ID (an empty GUID).
        /// </param>
        /// <returns>
        ///     Returns the newly instantiated <see cref="Estimate"/> object.
        ///     This object initially contains all-valid data.
        /// </returns>
        private Estimate InstantiateValidEstimateData(bool withValidId) =>
            new Estimate
            {
                Id = withValidId ? Guid.NewGuid() : default,
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
        /// <param name="canFindEstimates">
        ///     A flag indicating if <see cref="AppDbContext.Estimates"/> will return a valid instance when calling <see cref="DbSet{TEntity}.FindAsync(object[])"/>.
        ///     If set to <c>false</c>, the find method will always return <c>null</c>.
        ///     If set to <c>true</c>, the find method will always return a random <see cref="Estimate"/> instance (created by calling <see cref="InstantiateValidEstimateData(bool)"/>).
        /// </param>
        /// <returns>Returns the mocked <see cref="AppDbContext"/> instance.</returns>
        private AppDbContext CreateMockAppDbContext(Action<Mock<AppDbContext>> extraSetups = null, bool canFindEstimates = true)
        {
            // Create a mock DbSet<Estimate> object for the mocked AppDbContext
            Estimate usedObject = null;
            bool isUpdating = false;

            var estimatesDbSetMock = new Mock<DbSet<Estimate>>();
            estimatesDbSetMock.Setup(dbSet => dbSet.FindAsync(It.IsAny<object[]>()))
                .Callback<object[]>((objs) => {
                    isUpdating = true;
                    usedObject = InstantiateValidEstimateData(withValidId: false);
                    usedObject.Id = (Guid)objs[0];
                })
                .Returns(() => {
                    if (canFindEstimates == false)
                        return ValueTask.FromResult<Estimate>(null);

                    var result = isUpdating
                        ? usedObject
                        : InstantiateValidEstimateData(withValidId: true);
                    return ValueTask.FromResult(result);
                });


            // Create a mock AppDbContext and simulate some of its methods and properties
            var appDbContextMock = new Mock<AppDbContext>();
            appDbContextMock.Setup(context => context.AddAsync(It.IsAny<Estimate>(), It.IsAny<CancellationToken>()))
                .Callback<Estimate, CancellationToken>((addedEstimate, _) => {
                    usedObject = addedEstimate;
                });
            appDbContextMock.Setup(context => context.AddAsync(It.IsAny<Estimate>(), It.IsAny<CancellationToken>()))
                .Callback<Estimate, CancellationToken>((addedEstimate, _) => {
                    usedObject = addedEstimate;
                });
            appDbContextMock.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => {
                    // Assigns a new ID to the Estimate, as it happens when DbContext.SaveChangesAsync(...) is called
                    usedObject.Id = (usedObject.Id == Guid.Empty)
                        ? Guid.NewGuid()
                        : usedObject.Id;
                });

            appDbContextMock.SetupGet(context => context.Estimates)
                .Returns(estimatesDbSetMock.Object);


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
            var inputData = InstantiateValidEstimateData(withValidId: false);
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
            bool isValidOutputData = Validator.TryValidateObject(outputData, outputValidationContext, outputValidationResults);

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
            var inputData = InstantiateValidEstimateData(withValidId: false);
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
            var inputData = InstantiateValidEstimateData(withValidId: false);
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
            var inputData = InstantiateValidEstimateData(withValidId: false);

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
            var inputData = InstantiateValidEstimateData(withValidId: false);

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


        [Fact]
        public async Task GetEstimateByIdAsync_ExistingGuid_ReturnsEstimateDataSuccessfully()
        {
            // Arrange
            var mockAppDbContext = CreateMockAppDbContext(canFindEstimates: true);
            var estimatesService = new EstimatesService(mockAppDbContext);

            var someRandomGuid = Guid.NewGuid();

            // Act
            var result = await estimatesService.GetEstimateByIdAsync(someRandomGuid);

            // Assert
            Assert.NotNull(result);
        }


        [Fact]
        public async Task GetEstimateByIdAsync_NonExistingGuid_ReturnsNull()
        {
            // Arrange
            var mockAppDbContext = CreateMockAppDbContext(canFindEstimates: false);
            var estimatesService = new EstimatesService(mockAppDbContext);

            var someRandomGuid = Guid.NewGuid();

            // Act
            var result = await estimatesService.GetEstimateByIdAsync(someRandomGuid);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task UpdateEstimateAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var inputData = InstantiateValidEstimateData(withValidId: true);
            var inputValidationContext = new ValidationContext(inputData);
            var inputValidationResults = new List<ValidationResult>();

            var mockAppDbContext = CreateMockAppDbContext();
            var estimatesService = new EstimatesService(mockAppDbContext);


            // Act
            bool inputDataHasNonEmptyId = (inputData.Id != Guid.Empty);
            bool isValidInputData = Validator.TryValidateObject(inputData, inputValidationContext, inputValidationResults);

            var outputData = await estimatesService.UpdateEstimateAsync(inputData);
            var outputValidationContext = new ValidationContext(outputData);
            var outputValidationResults = new List<ValidationResult>();
            bool isValidOutputData = Validator.TryValidateObject(outputData, outputValidationContext, outputValidationResults);

            bool outputHasSameIdAsInput = (outputData.Id == inputData.Id);


            // Assert
            Assert.True(inputDataHasNonEmptyId);
            Assert.True(isValidInputData);
            Assert.True(isValidOutputData);
            Assert.True(outputHasSameIdAsInput);
        }


        [Fact]
        public async Task UpdateEstimateAsync_EmptyGuid_ThrowsServiceException()
        {
            // Arrange
            var inputData = InstantiateValidEstimateData(withValidId: false);

            var mockAppDbContext = CreateMockAppDbContext();
            var estimatesService = new EstimatesService(mockAppDbContext);


            // Act
            bool inputDataHasEmptyId = (inputData.Id == Guid.Empty);
            ServiceException caughtException = null;
            try
            {
                await estimatesService.UpdateEstimateAsync(inputData);
            }
            catch (ServiceException svcEx)
            {
                caughtException = svcEx;
            }


            // Assert
            Assert.True(inputDataHasEmptyId);
            Assert.NotNull(caughtException);
            Assert.NotNull(caughtException.ValidationErrors);
            Assert.NotEmpty(caughtException.ValidationErrors);
            Assert.Contains(caughtException.ValidationErrors.Keys, errorKey => errorKey == nameof(Estimate.Id));
        }


        [Fact]
        public async Task UpdateEstimateAsync_TargetEntityNotFound_ThrowsServiceException()
        {
            // Arrange
            var inputData = InstantiateValidEstimateData(withValidId: true);

            var mockAppDbContext = CreateMockAppDbContext(canFindEstimates: false);
            var estimatesService = new EstimatesService(mockAppDbContext);


            // Act
            bool inputDataHasNonEmptyId = (inputData.Id != Guid.Empty);
            ServiceException caughtException = null;
            try
            {
                await estimatesService.UpdateEstimateAsync(inputData);
            }
            catch (ServiceException svcEx)
            {
                caughtException = svcEx;
            }


            // Assert
            Assert.True(inputDataHasNonEmptyId);
            Assert.NotNull(caughtException);
            Assert.NotNull(caughtException.ValidationErrors);
            Assert.NotEmpty(caughtException.ValidationErrors);
            Assert.Contains(caughtException.ValidationErrors.Keys, errorKey => errorKey == nameof(Estimate.Id));
        }


        [Fact]
        public async Task UpdateEstimateAsync_SaveChangesAsyncThrowsException_ThrowsServiceException()
        {
            // Arrange
            var inputData = InstantiateValidEstimateData(withValidId: true);

            var mockAppDbContext = CreateMockAppDbContext(mockConfigs => {
                mockConfigs.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .Throws<Exception>();
            });
            var estimatesService = new EstimatesService(mockAppDbContext);


            // Act
            ServiceException caughtException = null;
            try
            {
                await estimatesService.UpdateEstimateAsync(inputData);
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
