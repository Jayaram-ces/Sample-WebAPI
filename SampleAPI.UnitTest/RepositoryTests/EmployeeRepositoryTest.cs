using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Constant;
using SampleAPI.Contexts;
using SampleAPI.Models;
using SampleAPI.Repository;
using Xunit;

namespace SampleAPI.UnitTest.RepositoryTests
{
    public class EmployeeRepositoryTest
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly EmployeeContext _dbContext;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<EmployeeContext>()
                            .UseInMemoryDatabase(databaseName: "EmployeeDB")
                            .Options;
            _dbContext = new EmployeeContext(options);
            _employeeRepository = new EmployeeRepository(_dbContext);
        }

        #region GetAllAsync
        [Fact(DisplayName = "ShouldReturnAllEmployees_WhenGetAllAsync_IsCalled_WithGivenParameters")]
        public async void ShouldReturnAllEmployees_WhenGetAllAsync_IsCalled()
        {
            //Arrange
            var expected = _fixture.Build<GetResponseModel<EmployeeModel>>().Create();
            var getEmployee = _fixture.Build<GetEmployeeParameters>().Without(x => x.Search).Without(x => x.FilterByRole).With(x => x.IncludeInActive, false).With(x => x.PageSize, 0).Create();
            await _dbContext.AddRangeAsync(expected.Data);
            await _dbContext.SaveChangesAsync();

            //Act
            var actual = await _employeeRepository.GetAllAsync(getEmployee);

            //Assert
            Assert.NotNull(actual);
            actual.Data.Where(x => x.IsActive).Should().BeEquivalentTo(expected.Data.Where(x => x.IsActive));
        }

        [Fact(DisplayName = "ShouldReturnEmployees_WhenGetAllAsync_IsCalled_ForSearch")]
        public async void SearchEmployees_With_GetAllAsync()
        {
            //Arrange
            var expected = _fixture.Build<GetResponseModel<EmployeeModel>>().Create();
            await _dbContext.AddRangeAsync(expected.Data);
            await _dbContext.SaveChangesAsync();
            var searchEmployee = expected.Data.FirstOrDefault();
            var expectedList = _fixture.CreateMany<EmployeeModel>(0).ToList();
            expectedList.Add(searchEmployee);
            var getEmployee = _fixture.Build<GetEmployeeParameters>().With(x => x.Search, searchEmployee.Name).Without(x => x.FilterByRole).With(x => x.PageSize, 0).Create();

            //Act
            var actual = await _employeeRepository.GetAllAsync(getEmployee);

            //Assert
            Assert.NotNull(actual);
            actual.Data.Should().BeEquivalentTo(expectedList);
        }

        [Fact(DisplayName = "ShouldReturnEmployees_WhenGetAllAsync_IsCalled_ForFilter_WithRole")]
        public async void FilterEmployees_With_GetAllAsync()
        {
            //Arrange
            string role = "Software Developer";
            var expected = _fixture.Build<GetResponseModel<EmployeeModel>>().With(x => x.Data, _fixture.Build<EmployeeModel>().With(x=>x.Role, role).CreateMany()).Create();
            var getEmployee = _fixture.Build<GetEmployeeParameters>().With(x => x.FilterByRole, role).Without(x => x.Search).With(x => x.PageSize, 0).Create();
            await _dbContext.AddRangeAsync(expected.Data);
            await _dbContext.SaveChangesAsync();

            //Act
            var actual = await _employeeRepository.GetAllAsync(getEmployee);

            //Assert
            Assert.NotNull(actual);
            actual.Data.Should().BeEquivalentTo(expected.Data.Where(x => x.IsActive));
        }
        #endregion

        #region GetAsync
        [Fact]
        public async void ShouldReturnEmployee_WhenGetAsync_IsCalled()
        {
            //Arrange
            var employeeList = _fixture.Build<EmployeeModel>().CreateMany().ToList();
            await _dbContext.AddRangeAsync(employeeList);
            await _dbContext.SaveChangesAsync();

            var expected = employeeList[new Random().Next(0, 2)];

            //Act
            var actual = await _employeeRepository.GetAsync(expected.Id);

            //Assert
            Assert.NotNull(actual);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ShouldReturnNull_WhenGetAsync_IsCalled_WithInvalidId()
        {
            //Act
            var excepted = await _employeeRepository.GetAsync(_fixture.Create<int>());

            //Assert
            Assert.Null(excepted);
        }
        #endregion

        #region AddAsync
        [Fact]
        public async void Should_AddEmployeeToDB_WhenAddAsync_IsCalled()
        {
            //Arrange
            var expected = _fixture.Build<EmployeeModel>().Create();

            //Act
            await _employeeRepository.AddAsync(expected);

            //Assert
            var actual = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == expected.Id);
            actual.Should().NotBeNull().And.BeEquivalentTo(expected);
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async void ShouldDeleteEmployee_WhenDeleteAsyncIsCalled()
        {
            //Arrange
            var expected = _fixture.Build<EmployeeModel>().Create();
            await _dbContext.AddRangeAsync(expected);
            await _dbContext.SaveChangesAsync();

            //Act
            await _employeeRepository.DeleteAsync(expected.Id);

            //Assert
            var actual = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == expected.Id);
            actual.Should().BeNull().And.NotBeSameAs(expected);
        }

        [Fact]
        public async void ShouldThrowError_WhenDeleteAsync_IsCalled_WithInValidId()
        {
            //Arrange
            var input = _fixture.Build<EmployeeModel>().With(x => x.Id, _fixture.Create<int>()).Create();

            //Act
            var act = _employeeRepository.DeleteAsync(input.Id);

            //Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => act);
            Assert.Equal(AppConstants.NoRecordErrorMessage, (exception as Exception).Message);
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async void ShouldUpdateEmployeeDetails_WhenUpdateAsync_IsCalled()
        {
            string updateName = _fixture.Create<string>();
            var expected = _fixture.Build<EmployeeModel>().Create();
            await _dbContext.AddRangeAsync(expected);
            await _dbContext.SaveChangesAsync();
            expected.Name = updateName;

            await _employeeRepository.UpdateAsync(expected);

            var actual = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == expected.Id);
            actual.Name.Should().NotBeNull().And.BeEquivalentTo(expected.Name);
        }
        #endregion
    }
}
