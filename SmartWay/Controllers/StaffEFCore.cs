using Microsoft.AspNetCore.Mvc;
using SmartWay.EFCore;
using SmartWay.ModelsDTO;
using SmartWay.Services;

namespace SmartWay.Controllers
{
    public class StaffEFCore : Controller
    {
        private readonly StaffEFCoreService _staffEFCoreService;

        public StaffEFCore(StaffEFCoreService staffEFCoreService)
        {
            _staffEFCoreService = staffEFCoreService;
        }

        [HttpPost("AddEmployees")]
        public async Task<IActionResult> AddEmployees([FromBody] List<EmployeeDTO> employees)
        {
            var Ids = await _staffEFCoreService.AddEmployees(employees);
            if (Ids is null)
            {
                return BadRequest("Ids is null");
            }
            return Ok(Ids);
        }

        [HttpDelete("DeleteEmployees")]
        public async Task<IActionResult> DeleteEmployees([FromBody] List<int> ids)
        {

            await _staffEFCoreService.DeleteEmployees(ids);
            return Ok();
        }

        [HttpGet("GetEmployeesByCompanyId")]
        public async Task<IActionResult> GetEmployeesByCompanyId([FromQuery] int companyId)
        {
            var employees = await _staffEFCoreService.GetEmployeesByCompanyId(companyId);

            if (employees is null)
            {
                return BadRequest();
            }
            return Ok(employees);
        }

        [HttpGet("GetEmployeesByDepartmentName")]
        public async Task<IActionResult> GetEmployeesByDepartmentName([FromQuery] string departmentName)
        {
            var employees = await _staffEFCoreService.GetEmployeesByDepartmentName(departmentName);

            if (employees is null) 
            {
                return BadRequest();
            }

            return Ok(employees);
        }
        [HttpPatch("UpdateEmployeeInfo")]
        public async Task<IActionResult> UpdateEmployeeInfo([FromQuery] int employeeId, [FromBody] EmployeeDTO employee)
        {

            await _staffEFCoreService.UpdateEmployeeInfo(employeeId, employee);

            return Ok();
        }

    }
}
