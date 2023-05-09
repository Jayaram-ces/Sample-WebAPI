using SampleAPI.Models;

namespace SampleAPI.Repository
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeeModel>> GetAllAsync(string searchText, string role, bool includeInActive);
        Task<EmployeeModel> GetAsync(int id);
        Task UpdateAsync(EmployeeModel employeeModel);
        Task AddAsync(EmployeeModel employeeModel);
        Task DeleteAsync(int id);
    }
}
