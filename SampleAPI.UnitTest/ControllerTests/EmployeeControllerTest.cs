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
        private readonly EmployeeController _employeeController;

        public EmployeeControllerTest()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<EmployeeController>>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _employeeController = new EmployeeController(_loggerMock.Object, _employeeRepositoryMock.Object);
        }

        #region GetAllEmployee 

        [Fact(DisplayName = "ShouldReturn_500Error_WhenReposistoryThrowsException_WhileGetEmployeeIsCalled")]
        public async Task WhenReposistoryThrowsException_Returns_500Error_In_GetEmployee()
        {
            //Arrange;
            var getEmployeeParameters = _fixture.Create<GetEmployeeParameters>();
            _employeeRepositoryMock.Setup(x => x.GetAllAsync(getEmployeeParameters)).ThrowsAsync(_fixture.Create<Exception>());

            //Act
            var actual = await _employeeController.GetEmployee(getEmployeeParameters) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.GetAllAsync(getEmployeeParameters), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "ShouldReturn_AllEmployee_WhenGetEmployeeIsCalled_With_PageSize_0")]
        public async Task GivenPaginationParameters_ReturnsAllEmployee()
        {
            //Arrange
            var getEmployeeParameters = _fixture.Build<GetEmployeeParameters>().With(x => x.PageSize, 0).Create();
            var expected = _fixture.Create<GetResponseModel<EmployeeModel>>();
            _employeeRepositoryMock.Setup(x => x.GetAllAsync(getEmployeeParameters)).ReturnsAsync(expected);

            //Act
            var actual = await _employeeController.GetEmployee(getEmployeeParameters) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(expected, actual.Value);
            _employeeRepositoryMock.Verify(m => m.GetAllAsync(getEmployeeParameters), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "ShouldReturn_EmployeeRecords_With_PageSize_Of_20_PageNumber_Of_5_WhenGetEmployeeIsCalled_WithGivenPaginationParameter")]
        public async Task GivenPaginationParameters_ReturnsEmployeeList_In_FifthPage()
        {
            //Arrange
            var getEmployeeParameters = _fixture.Build<GetEmployeeParameters>().With(x => x.PageSize, 20).With(x => x.PageNumber, 5).Create();
            var expected = _fixture.Create<GetResponseModel<EmployeeModel>>();
            _employeeRepositoryMock.Setup(x => x.GetAllAsync(getEmployeeParameters)).ReturnsAsync(expected);

            //Act
            var actual = await _employeeController.GetEmployee(getEmployeeParameters) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(expected, actual.Value);
            _employeeRepositoryMock.Verify(m => m.GetAllAsync(getEmployeeParameters), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        #endregion

        #region GetEmployeeById
        [Fact(DisplayName = "ShouldReturn_500Error_WhenReposistoryThrowsException_WhileGetEmployeeByIdIsCalled")]
        public async Task ReposistoryThrowsException_Return_500Error_In_GetEmployeeById()
        {
            //Arrange
            var request = _fixture.Create<int>();
            _employeeRepositoryMock.Setup(x => x.GetAsync(request)).ThrowsAsync(_fixture.Create<Exception>());

            //Act
            var actual = await _employeeController.GetEmployeeById(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.GetAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "ShouldReturn_EmployeeRecord_WhenGetEmployeeByIdIsCalled_WithId")]
        public async Task GivenParameterInGetEmployeeById_ReturnsEmployeeRecord()
        {
            //Arrange
            var expected = _fixture.Create<EmployeeModel>();
            _employeeRepositoryMock.Setup(x => x.GetAsync(expected.Id)).ReturnsAsync(expected);

            //Act
            var actual = await _employeeController.GetEmployeeById(expected.Id) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(expected, actual.Value);
            _employeeRepositoryMock.Verify(m => m.GetAsync(expected.Id), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "ShouldReturn_404_NotFound_When_EmployeeRepositoryReturnNull_WhileGetEmployeeById_IsCalled")]
        public async Task ReposistoryReturnsNull_Returns_404_NotFound_In_GetEmployeeById()
        {
            //Arrange
            var request = _fixture.Create<int>();
            _employeeRepositoryMock.Setup(x => x.GetAsync(request)).ReturnsAsync((EmployeeModel)null);

            //Act
            var actual = await _employeeController.GetEmployeeById(request) as NotFoundResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.GetAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }
        #endregion

        #region AddEmployee

        [Fact(DisplayName = "ShouldReturn_500Error_WhenReposistoryThrowsException_WhileAddEmployeeIsCalled")]
        public async Task ReposistoryThrowsException_Return_500Error_In_AddEmployee()
        {
            //Arrange
            var request = _fixture.Create<EmployeeModel>();
            _employeeRepositoryMock.Setup(x => x.AddAsync(request)).ThrowsAsync(_fixture.Create<Exception>());

            //Act
            var actual = await _employeeController.AddEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.AddAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        public async void ShouldAddEmploye_WhenAddEmployeeIsCalled()
        {
            //Arrange
            var request = _fixture.Create<EmployeeModel>();

            //Act
            var actual = await _employeeController.AddEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(AppConstants.AddSuccessMessage, (actual.Value as UpdateResponseModel).StatusMessage);
            _employeeRepositoryMock.Verify(m => m.AddAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        #endregion

        #region UpdateEmployee

        [Fact(DisplayName = "ShouldReturn_500Error_WhenReposistoryThrowsException_WhileUpdateEmployeeIsCalled")]
        public async Task ReposistoryThrowsException_Return_500Error_In_UpdateEmployee()
        {
            //Arrange
            var request = _fixture.Create<EmployeeModel>();
            _employeeRepositoryMock.Setup(x => x.UpdateAsync(request)).ThrowsAsync(_fixture.Create<Exception>());

            //Act
            var actual = await _employeeController.UpdateEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.UpdateAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldUpdateEmployee_WhenUpdateEmployeeIsCalled()
        {
            //Arrange
            var request = _fixture.Create<EmployeeModel>();

            //Act
            var actual = await _employeeController.UpdateEmployee(request) as ObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(AppConstants.UpdateSuccessMessage, (actual.Value as UpdateResponseModel).StatusMessage);
            _employeeRepositoryMock.Verify(m => m.UpdateAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }
        #endregion

        #region DeleteEmployee

        [Fact(DisplayName = "ShouldReturn_404_NotFound_WhenDeleteEmployeeIsCalled_WithInvalidEmployeeId")]
        public async Task GivenInvalidId_Return_404_In_DeleteEmployee()
        {
            //Arrange
            var request = _fixture.Create<int>();
            var exception = _fixture.Build<CustomException>().With(x => x.Message, AppConstants.NoRecordErrorMessage).Create();
            _employeeRepositoryMock.Setup(x => x.DeleteAsync(request)).Throws(exception);

            //Act
            var actual = await _employeeController.DeleteEmployee(request) as StatusCodeResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            _employeeRepositoryMock.Verify(m => m.DeleteAsync(request), Times.Once);
            _employeeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "ShouldReturn_500Error_WhenReposistoryThrowsException_WhileDeleteEmployeeIsCalled")]
        public async Task ReposistoryThrowsException_Return_500Error_In_DeleteEmployee()
        {
            //Arrange
            var request = _fixture.Create<int>();
            _employeeRepositoryMock.Setup(x => x.DeleteAsync(request)).ThrowsAsync(_fixture.Create<Exception>());

            //Act
            var actual = await _employeeController.DeleteEmployee(request) as ObjectResult;

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
            var actual = await _employeeController.DeleteEmployee(request) as ObjectResult;

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
