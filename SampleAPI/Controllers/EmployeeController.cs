using Microsoft.AspNetCore.Mvc;
using SampleAPI.Constant;
using SampleAPI.Models;
using SampleAPI.Repository;

namespace SampleAPI.Controllers
{
    /// <summary>
    /// EmployeeModel controller is used to perform CRUD operations with employee entity.
    /// </summary>
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAllEmployee()
        {
            try
            {
                var result = await _repository.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(AppConstants.NoRecordFoundErrorMessage);
                _logger.LogCritical(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.NoRecordFoundErrorMessage);
            }
        }

        /// <summary>
        /// Get employee by Id.
        /// </summary>
        /// <param name="employeeId">Unique identifier of the employee.</param>
        /// <returns>Returns employee model.</returns>
        [HttpGet]
        [Route("GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            try
            {
                var employee = await _repository.GetAsync(employeeId);

                if (employee == null)
                {
                    return NotFound(AppConstants.NoSuchRecordFoundErrorMessage);
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(AppConstants.NoSuchRecordFoundErrorMessage);
                _logger.LogCritical(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.NoSuchRecordFoundErrorMessage);
            }
        }

        /// <summary>
        /// Add a new employee record.
        /// </summary>
        /// <param name="employee">EmployeeModel model.</param>
        /// <returns>Create status.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [Route("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeModel employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _repository.AddAsync(employee);
                return Ok(AppConstants.AddSuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(AppConstants.AddFailedErrorMessage);
                _logger.LogCritical(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _repository.UpdateAsync(employeeModel);
                return Ok(AppConstants.UpdateSuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(AppConstants.NoRecordFoundErrorMessage);
                _logger.LogCritical(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.NoRecordFoundErrorMessage);
            }
        }

        /// <summary>
        /// Delete the employee record.
        /// </summary>
        /// <param name="employeeId">Unique identifier of the employee.</param>
        /// <returns>Delete status</returns>
        [HttpDelete]
        [Route("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            try
            {
                await _repository.DeleteAsync(employeeId);
                return Ok(AppConstants.DeleteSuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(AppConstants.NoSuchRecordFoundErrorMessage);
                _logger.LogCritical(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.NoSuchRecordFoundErrorMessage);
            }
        }

        /// <summary>
        /// Search employee by name/ Role.
        /// </summary>
        /// <param name="searchText">Search text</param>
        /// <returns>EmployeeModel list related to search text.</returns>
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> Search(string searchText)
        {
            try
            {
                var result = await _repository.SearchAsync(searchText);
                if (result != null && result is IEnumerable<EmployeeModel> employeeList && employeeList.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return Ok(AppConstants.NoSuchRecordFoundWithSearchTextErrorMessage);

                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(AppConstants.NoSuchRecordFoundWithSearchTextErrorMessage);
                _logger.LogCritical(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.NoSuchRecordFoundWithSearchTextErrorMessage);
            }
        }

        /// <summary>
        /// Filter employee by Role and IsActive
        /// </summary>
        /// <param name="role">EmployeeModel role.</param>
        /// <param name="isActive">EmployeeModel is Active or not.</param>
        /// <returns>Filter employee based on selected filter.</returns>
        [HttpGet]
        [Route("Filter")]
        public async Task<IActionResult> Filter(string role, bool isActive)
        {
            try
            {
                var result = await _repository.FilterAsync(role, isActive);
                if (result != null && result is IEnumerable<EmployeeModel> employeeList && employeeList.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return Ok(AppConstants.NoRecordFoundErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(AppConstants.NoRecordFoundErrorMessage);
                _logger.LogCritical(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, AppConstants.NoRecordFoundErrorMessage);
            }
        }
    }
}
