using Microsoft.AspNetCore.Mvc;
using SampleAPI.Constant;
using SampleAPI.Models;
using SampleAPI.Repository;

namespace SampleAPI.Controllers
{
    /// <summary>
    /// EmployeeModel controller is used to perform CRUD operations with employee entity.
    /// </summary>
    [Route("sampleAPI/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeRepository _repository;
        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <summary>
        /// Get all employee records.
        /// </summary>
        /// <returns>Returns the employee list.</returns>
        [HttpGet]
        [Route("GetAllEmployee")]
        public async Task<IActionResult> GetEmployee([FromQuery] GetEmployeeParameters pagingparametermodel)
        {
            try
            {
                var response = new GetResponseModel<EmployeeModel>();
                IEnumerable<EmployeeModel> result = new List<EmployeeModel>();
                var getEmployees = (await _repository.GetAllAsync(pagingparametermodel)).ToList();

                int TotalPages = 1;
                int totalRecordCount = getEmployees.Count;
                int CurrentPage = pagingparametermodel.PageNumber;
                int PageSize = pagingparametermodel.PageSize;

                if (PageSize > 0)
                {
                    TotalPages = (int)Math.Ceiling(totalRecordCount / (double)PageSize);
                    result = getEmployees.OrderBy(x => x.Name).Skip((CurrentPage - 1) * PageSize).Take(PageSize);
                }
                else
                {
                    result = getEmployees;
                }

                response.TotalRecordCount = totalRecordCount;
                response.TotalPages = TotalPages;
                response.PageSize = PageSize;
                response.CurrentPage = CurrentPage;
                response.HasPreviousPage = CurrentPage > 1;
                response.HasNextPage = CurrentPage < TotalPages;
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.CommonErrorMessage);
            }
        }

        /// <summary>
        /// Get employee by Id.
        /// </summary>
        /// <param name="employeeId">Unique identifier of the employee.</param>
        /// <returns>Returns employee model.</returns>
        [HttpGet]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            try
            {
                var employee = await _repository.GetAsync(employeeId);

                if (employee == null)
                {
                    return NotFound();
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.CommonErrorMessage);
            }
        }

        /// <summary>
        /// Add a new employee record.
        /// </summary>
        /// <param name="employee">EmployeeModel model.</param>
        /// <returns>Create status.</returns>
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeModel employee)
        {
            try
            {
                await _repository.AddAsync(employee);
                return Ok(new UpdateResponseModel { Status = true, StatusMessage = AppConstants.AddSuccessMessage });
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.CommonErrorMessage);
            }
        }

        /// <summary>
        /// Update the employee record.
        /// </summary>
        /// <param name="id">Unique identifier of the employee.</param>
        /// <param name="employee">EmployeeModel model.</param>
        /// <returns>Update status.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(EmployeeModel employeeModel)
        {
            try
            {
                await _repository.UpdateAsync(employeeModel);
                return Ok(new UpdateResponseModel { Status = true, StatusMessage = AppConstants.UpdateSuccessMessage });
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.CommonErrorMessage);
            }
        }

        /// <summary>
        /// Delete the employee record.
        /// </summary>
        /// <param name="employeeId">Unique identifier of the employee.</param>
        /// <returns>Delete status</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            try
            {
                await _repository.DeleteAsync(employeeId);
                return Ok(new UpdateResponseModel { Status = true, StatusMessage = AppConstants.DeleteSuccessMessage });
            }
            catch (CustomException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.CommonErrorMessage);
            }
        }
    }
}
