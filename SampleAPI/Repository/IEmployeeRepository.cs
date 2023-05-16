using SampleAPI.Models;

namespace SampleAPI.Repository
{
    public interface IEmployeeRepository
    {
        Task<GetResponseModel<EmployeeModel>> GetAllAsync(GetEmployeeParameters getEmployeeParameters);
        Task<EmployeeModel> GetAsync(int id);
        Task UpdateAsync(EmployeeModel employeeModel);
        Task AddAsync(EmployeeModel employeeModel);
        Task DeleteAsync(int id);
    }
}
