using System.Data;
using System.Drawing.Printing;
using System.Linq;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleAPI.Constant;
using SampleAPI.Controllers;
using SampleAPI.Models;
using SampleAPI.Repository;
using Xunit;

namespace SampleAPI.UnitTest.ControllerTests
{
    public class EmployeeControllerTest
    {
        public readonly Fixture _fixture;
        private readonly Mock<ILogger<EmployeeController>> _loggerMock;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;

        public EmployeeControllerTest()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<EmployeeController>>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        }

        private EmployeeController CreateController()
        {
            return new EmployeeController(_loggerMock.Object, _employeeRepositoryMock.Object);
        }

        #region GetAllEmployee 

        [Fact]
        public async Task ShouldReturn_500InternalServerErrorMessage_WhenEmployeeReposistoryThrowException_WhileGetEmployeeIsCalled()
        {
            //Arrange
            var search = _fixture.Create<string>();
            var role = _fixture.Create<string>();
            var getEmployeeParameters = _fixture.Create<GetEmployeeParameters>();
            _employeeRepositoryMock.Setup(x => x.GetAllAsync(getEmployeeParameters)).ThrowsAsync(_fixture.Create<Exception>());
            var controller = CreateController();

            //Act
            var actual = await controller.GetEmployee(getEmployeeParameters) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.GetAllAsync(getEmployeeParameters), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturn_AllEmployee_WhenGetEmployeeIsCalled()
        {
            //Arrange
            var search = _fixture.Create<string>();
            var role = _fixture.Create<string>();
            var getEmployeeParameters = _fixture.Build<GetEmployeeParameters>().With(x=>x.PageSize, 0).Create();
            var expected = _fixture.Create<IEnumerable<EmployeeModel>>().AsQueryable();
            _employeeRepositoryMock.Setup(x => x.GetAllAsync(getEmployeeParameters)).ReturnsAsync(expected);
            var controller = CreateController();

            //Act
            var actual = await controller.GetEmployee(getEmployeeParameters) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(expected, (actual.Value as GetResponseModel<EmployeeModel>).Data);
            _employeeRepositoryMock.Verify(m => m.GetAllAsync(getEmployeeParameters), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturn_EmployeeRecords_With_PageSize_Of_20_PageNumber_Of_5WhenGetEmployeeIsCalled_Pagination()
        {
            //Arrange
            var search = _fixture.Create<string>();
            var role = _fixture.Create<string>();
            var getEmployeeParameters = _fixture.Build<GetEmployeeParameters>().With(x => x.PageSize, 20).With(x=>x.PageNumber, 5).Create();
            var emoployeeList = _fixture.CreateMany<EmployeeModel>(100).AsQueryable();
            var expected = emoployeeList.OrderBy(x => x.Name).Skip((getEmployeeParameters.PageNumber - 1) * getEmployeeParameters.PageSize).Take(getEmployeeParameters.PageSize).ToList();
            _employeeRepositoryMock.Setup(x => x.GetAllAsync(getEmployeeParameters)).ReturnsAsync(emoployeeList);
            var controller = CreateController();

            //Act
            var actual = await controller.GetEmployee(getEmployeeParameters) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(expected, (actual.Value as GetResponseModel<EmployeeModel>).Data);
            _employeeRepositoryMock.Verify(m => m.GetAllAsync(getEmployeeParameters), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        #endregion

        #region GetEmployeeById
        [Fact]
        public async Task ShouldReturn_500InternalServerErrorMessage_WhenEmployeeReposistoryThrowException_WhileGetEmployeeByIdIsCalled()
        {
            //Arrange
            var request = _fixture.Create<int>();
            _employeeRepositoryMock.Setup(x => x.GetAsync(request)).ThrowsAsync(_fixture.Create<Exception>());

            //Act
            var actual = await CreateController().GetEmployeeById(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.GetAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturn_EmployeeModel_WhenGetEmployeeByIdIsCalled()
        {
            //Arrange
            var expected = _fixture.Create<EmployeeModel>();
            _employeeRepositoryMock.Setup(x => x.GetAsync(expected.Id)).ReturnsAsync(expected);

            //Act
            var actual = await CreateController().GetEmployeeById(expected.Id) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(expected, actual.Value);
            _employeeRepositoryMock.Verify(m => m.GetAsync(expected.Id), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturn_404_NotFound_When_EmployeeRepositoryReturnNull_WhileGetEmployeeById_IsCalled()
        {
            //Arrange
            var request = _fixture.Create<int>();
            _employeeRepositoryMock.Setup(x => x.GetAsync(request)).ReturnsAsync((EmployeeModel)null);

            //Act
            var actual = await CreateController().GetEmployeeById(request) as NotFoundResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.GetAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }
        #endregion

        #region AddEmployee

        [Fact]
        public async Task ShouldReturn_500InternalServerErrorMessage_WhenEmployeeReposistoryThrowsException_WhileAddEmployeeIsCalled()
        {
            //Arrange
            var request = _fixture.Create<EmployeeModel>();
            _employeeRepositoryMock.Setup(x => x.AddAsync(request)).ThrowsAsync(_fixture.Create<Exception>());

            //Act
            var actual = await CreateController().AddEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.AddAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldAddEmploye_WhenAddEmployeeIsCalled()
        {
            //Arrange
            var request = _fixture.Create<EmployeeModel>();

            //Act
            var actual = await CreateController().AddEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(AppConstants.AddSuccessMessage, (actual.Value as UpdateResponseModel).StatusMessage);
            _employeeRepositoryMock.Verify(m => m.AddAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        #endregion

        #region UpdateEmployee

        [Fact]
        public async Task ShouldReturn_500InternalServerErrorMessage_WhenEmployeeReposistoryThrowsException_WhileUpdateEmployeeIsCalled()
        {
            //Arrange
            var request = _fixture.Create<EmployeeModel>();
            _employeeRepositoryMock.Setup(x => x.UpdateAsync(request)).ThrowsAsync(_fixture.Create<Exception>());

            //Act
            var actual = await CreateController().UpdateEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.UpdateAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldUpdateEmployeeDetails_WhenUpdateEmployeeIsCalled()
        {
            //Arrange
            var request = _fixture.Create<EmployeeModel>();

            //Act
            var actual = await CreateController().UpdateEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(AppConstants.UpdateSuccessMessage, (actual.Value as UpdateResponseModel).StatusMessage);
            _employeeRepositoryMock.Verify(m => m.UpdateAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }
        #endregion

        #region DeleteEmployee

        [Fact]
        public async Task ShouldReturn_204_NoContent_WhenDeleteEmployeeIsCalled_WithInvalidEmployeeId()
        {
            //Arrange
            var request = _fixture.Create<int>();
            var exception = _fixture.Build<CustomException>().With(x => x.Message, AppConstants.NoRecordErrorMessage).Create();
            _employeeRepositoryMock.Setup(x => x.DeleteAsync(request)).Throws(exception);

            //Act
            var actual = await CreateController().DeleteEmployee(request) as StatusCodeResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status204NoContent, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.DeleteAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturn_500InternalServerErrorMessage_WhenEmployeeReposistoryThrowsException_WhileDeleteEmployeeIsCalled()
        {
            //Arrange
            var request = _fixture.Create<int>();
            _employeeRepositoryMock.Setup(x => x.DeleteAsync(request)).ThrowsAsync(_fixture.Create<Exception>());

            //Act
            var actual = await CreateController().DeleteEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.DeleteAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldDeleteEmployee_WhenDeleteEmployeeIsCalled()
        {
            //Arrange
            var request = _fixture.Create<int>();

            //Act
            var actual = await CreateController().DeleteEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(AppConstants.DeleteSuccessMessage, (actual.Value as UpdateResponseModel).StatusMessage);
            _employeeRepositoryMock.Verify(m => m.DeleteAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }
        #endregion
    }
}
