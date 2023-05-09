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
        public async Task<IActionResult> GetEmployee(string? search, string? filterByRole, bool includeInActive, [FromQuery] PagingParameterModel pagingparametermodel)
        {
            try
            {
                var response = new GetResponseModel<EmployeeModel>();
                var result = await _repository.GetAllAsync(search, filterByRole, includeInActive);

                int TotalPages = 0;
                int totalRecordCount = result.Count();
                int CurrentPage = pagingparametermodel.PageNumber;
                int PageSize = pagingparametermodel.PageSize;

                if (PageSize > 0)
                {
                    TotalPages = (int)Math.Ceiling(totalRecordCount / (double)PageSize);
                    result = result.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
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
        [Route("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            try
            {
                var employee = await _repository.GetAsync(employeeId);

                if (employee == null)
                {
                    return NotFound(AppConstants.NoRecordErrorMessage);
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
        [Route("AddEmployee")]
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
        [Route("UpdateEmployee")]
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
        [Route("{employeeId}")]
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
            catch(Exception ex)
            {
                _logger.LogInformation(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.CommonErrorMessage);
            }
        }
    }
}
