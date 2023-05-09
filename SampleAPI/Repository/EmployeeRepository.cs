using Microsoft.EntityFrameworkCore;
using SampleAPI.Constant;
using SampleAPI.Contexts;
using SampleAPI.Models;

namespace SampleAPI.Repository
{
    /// <summary>
    /// EmployeeRepository is used to perform CRUD operations with employee entity.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeContext _dbContext;

        public EmployeeRepository(EmployeeContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add new employee record to the database.
        /// </summary>
        /// <param name="employeeModel">Employee model</param>
        public async Task AddAsync(EmployeeModel employeeModel)
        {
            if (employeeModel != null)
            {
                _dbContext.Employees.Add(employeeModel);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Delete employee from the database.
        /// </summary>
        /// <param name="employeeId">Employee model.</param>
        public async Task DeleteAsync(int employeeId)
        {
            var employee = await _dbContext.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                throw new CustomException(AppConstants.NoRecordErrorMessage);
            }

            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get the employee list.
        /// </summary>
        /// <returns>Employee list.</returns>
        public async Task<IEnumerable<EmployeeModel>> GetAllAsync(string searchText, string role, bool includeInActive)
        {
             var employeeList = await _dbContext.Employees.ToListAsync();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                employeeList = employeeList.Where(x => x.Name.Contains(searchText) || x.Role.Contains(searchText)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                employeeList = employeeList.Where(x => x.Role.Equals(role)).ToList();
            }

            if (!includeInActive)
            {
                employeeList = employeeList.Where(x => x.IsActive).ToList();
            }

            return employeeList;
        }

        /// <summary>
        /// Get employee by Id.
        /// </summary>
        /// <param name="id">Employee Id</param>
        /// <returns>Employee model</returns>
        public async Task<EmployeeModel> GetAsync(int id)
        {
            return await _dbContext.Employees.FindAsync(id);
        }

        /// <summary>
        /// Update employee details.
        /// </summary>
        /// <param name="employeeModel">Employee model.</param>
        public async Task UpdateAsync(EmployeeModel employeeModel)
        {
            _dbContext.Entry(employeeModel).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
