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
            if (_dbContext != null)
            {
                var employee = await _dbContext.Employees.FindAsync(employeeId);

                if (employee == null)
                {
                    throw new Exception(AppConstants.NoSuchRecordFoundErrorMessage);
                }

                if (employee != null)
                {
                    _dbContext.Employees.Remove(employee);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Filter employee by Role and IsActive.
        /// </summary>
        /// <param name="role">Employee role</param>
        /// <param name="isActive">Employee status.</param>
        public async Task<IEnumerable<EmployeeModel>> FilterAsync(string role, bool isActive)
        {
            if (_dbContext != null && _dbContext.Employees.Any())
            {
                return await _dbContext.Employees.Where(x => x.Role.Contains(role) && x.IsActive.Equals(isActive)).ToListAsync();
            }
            return null;
        }

        /// <summary>
        /// Get the employee list.
        /// </summary>
        /// <returns>Employee list.</returns>
        public async Task<IEnumerable<EmployeeModel>> GetAllAsync()
        {
            if (_dbContext != null && _dbContext.Employees.Any())
            {
                return await _dbContext.Employees.ToListAsync();
            }

            return null;
        }

        /// <summary>
        /// Get employee by Id.
        /// </summary>
        /// <param name="id">Employee Id</param>
        /// <returns>Employee model</returns>
        public async Task<EmployeeModel> GetAsync(int id)
        {
            if (_dbContext.Employees != null && _dbContext.Employees.Any())
            {
                return await _dbContext.Employees.FindAsync(id);
            }

            return null;
        }

        /// <summary>
        /// Search employee from database using search keyword.
        /// </summary>
        /// <param name="searchText">Search text.</param>
        /// <returns>Employee list.</returns>
        public async Task<IEnumerable<EmployeeModel>> SearchAsync(string searchText)
        {
            if (_dbContext != null && _dbContext.Employees.Any())
            {
                return await _dbContext.Employees.Where(x => x.Name.Contains(searchText) || x.Role.Contains(searchText)).ToListAsync();
            }

            return null;
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
