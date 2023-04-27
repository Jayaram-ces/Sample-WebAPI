using SampleAPI.Models;

namespace SampleAPI.Repository
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeeModel>> GetAllAsync();
        Task<EmployeeModel> GetAsync(int id);
        Task UpdateAsync(EmployeeModel employeeModel);
        Task AddAsync(EmployeeModel employeeModel);
        Task DeleteAsync(int id);
        Task<IEnumerable<EmployeeModel>> SearchAsync(string searchText);
        Task<IEnumerable<EmployeeModel>> FilterAsync(string role, bool isActive);
    }
}
