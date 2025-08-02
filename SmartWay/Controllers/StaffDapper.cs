using Microsoft.AspNetCore.Mvc;
using SmartWay.Postgres;
using SmartWay.Postgres.Interfaces;
using SmartWay.Postgres.Models;
using SmartWay.Helpers;


namespace SmartWay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StaffDapper : ControllerBase
    {

        private readonly ILogger<StaffDapper> _logger;
        private readonly DbContext _dbContext;

        public StaffDapper(ILogger<StaffDapper> logger, DbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost(Name = "AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            try
            {
                int? employeeId = await _dbContext.AddEmployee(employee);
                if (employeeId is null)
                {
                    return BadRequest("We can't Add Employee");
                }
                return Ok(employeeId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee([FromBody] int Id)
        {
            // Реализовано "жёсткое удаление"
            try
            {
                await _dbContext.DeleteEmployee(Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("companyId")]
        public async Task<IActionResult> GetAllEmployeeFromCompany([FromQuery] int companyId)
        {
            try
            {
                var employees = await _dbContext.GetAllEmployeeFromCompany(companyId);
                if (employees is null)
                {
                    return BadRequest("Employees are null");
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("departmentName")]
        public async Task<IActionResult> GetAllEmployeeFromDepartment([FromQuery] string departmentName)
        {
            try
            {
               
                var employees = await _dbContext.GetAllEmployeeFromDepartment(departmentName);
                if (employees is null)
                {
                    return BadRequest("Employees are null");
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("UpdateEmployeeInfo")]
        public async Task<IActionResult> UpdateEmployeeInfo(int Id, [FromBody] Employee employee)
        {
            try
            {
                // Парсинг/Проверка на string.Empty у employee
                StaffService.ParseEmployee(employee);
                await _dbContext.UpdateEmployeeInfo(Id, employee);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
