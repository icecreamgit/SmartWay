using Microsoft.EntityFrameworkCore;
using SmartWay.EFCore;
using SmartWay.Postgres.Interfaces;
using SmartWay.ModelsDTO;
using SmartWay.Postgres.Models;
using System.Security.Cryptography.Xml;

namespace SmartWay.Services
{
    public class StaffEFCoreService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILoggerEasy _loggerEasy;
        public StaffEFCoreService(AppDbContext dbContext, ILoggerEasy loggerEasy)
        {
            _dbContext = dbContext;
            _loggerEasy = loggerEasy;
        }

        public async Task<List<int>> AddEmployees(List<EmployeeDTO> employees)
        {
            try
            {
                var _employees = new List<Employee>();

                foreach(var employeeDto in employees)
                {
                    var _employee = new Employee()
                    {
                        Name = employeeDto.Name,
                        Surname = employeeDto.Surname,
                        Phone = employeeDto.Phone,
                        CompanyId = employeeDto.CompanyId,
                        Passport = new Passport()
                        {
                            Type = employeeDto.Passport.Type,
                            Number = employeeDto.Passport.Number
                        },
                        Department = new Department()
                        {
                            Name = employeeDto.Department.Name,
                            Phone = employeeDto.Department.Phone
                        }
                    };
    
                    _employees.Add(_employee);

                }

                await _dbContext.Employees.AddRangeAsync(_employees);
                await _dbContext.SaveChangesAsync();

                // Если не упало, то логично, что всё ок, тянем данные из employees
                List<int> ids = _employees.Select(xx => xx.Id).ToList();

                return ids;
            }
            catch (Exception ex)
            {
                _loggerEasy.Error(ex.ToString());
                return null;
            }
            
        }

    
        public async Task DeleteEmployees(List<int> ids)
        {
            try
            {
                var employeesToDelete = await _dbContext.Employees
                .Where(xx => ids.Contains(xx.Id))
                .ToListAsync();

                _dbContext.RemoveRange(employeesToDelete);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _loggerEasy.Error(ex.Message);
            }
            
        }

        public async Task<List<EmployeeDTO>> GetEmployeesByCompanyId(int companyId)
        {
            try
            {
                var employeesList = await _dbContext.Employees
                .Where(xx => xx.CompanyId == companyId)
                .Include(e => e.Passport)
                .Include(e => e.Department)
                .ToListAsync();

                var employeesResult = employeeDTOs(employeesList);
                return employeesResult;
            }
            catch(Exception ex)
            {
                _loggerEasy.Error(ex.ToString());
                return null;
            }
            
        }

        public async Task<List<EmployeeDTO>> GetEmployeesByDepartmentName(string departmentName)
        {
            try
            {
                var employees = await _dbContext.Employees
                    .Include(xx => xx.Passport)
                    .Include(xx => xx.Department)
                    .Where(xx => xx.Department.Name == departmentName)
                    .OrderBy(xx => xx.CompanyId)
                    .ToListAsync();

                var employeesResult = employeeDTOs(employees);

                return employeesResult;
            }
            catch(Exception ex) 
            {
                _loggerEasy.Error(ex.Message);
                return null;
            }
        }

        public async Task UpdateEmployeeInfo(int employeeId, EmployeeDTO employeeDto)
        {
            try
            {
                var employeeFinded = await _dbContext.Employees
                    .Include(xx => xx.Passport)
                    .Include(xx => xx.Department)
                    .FirstOrDefaultAsync(xx => xx.Id == employeeId);
                if (employeeFinded is null)
                    throw new Exception("employeeFinded is null");

                // Ручной маппинг
                employeeFinded.Name = employeeDto.Name == string.Empty ? employeeFinded.Name : employeeDto.Name;
                employeeFinded.Surname = employeeDto.Surname == string.Empty ? employeeFinded.Surname : employeeDto.Surname;
                employeeFinded.Phone = employeeDto.Phone == string.Empty ? employeeFinded.Phone : employeeDto.Phone;
                employeeFinded.CompanyId = employeeDto.CompanyId == 0 ? employeeFinded.CompanyId : employeeDto.CompanyId;
                employeeFinded.Passport.Type = employeeFinded.Passport.Type == string.Empty ? employeeFinded.Passport.Type : employeeDto.Passport.Type;
                employeeFinded.Passport.Number = employeeFinded.Passport.Number == string.Empty ? employeeFinded.Passport.Number : employeeDto.Passport.Number;
                employeeFinded.Department.Name = employeeFinded.Department.Name == string.Empty ? employeeFinded.Department.Name : employeeDto.Department.Name;
                employeeFinded.Department.Phone = employeeFinded.Department.Phone == string.Empty ? employeeFinded.Department.Phone : employeeDto.Department.Phone;

                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _loggerEasy.Error(ex.Message);
            }
        }


        protected List<EmployeeDTO> employeeDTOs (List<Employee> employees)
        {
            var employeesResult = new List<EmployeeDTO>();
            foreach (var employee in employees)
            {
                var employeeLocal = new EmployeeDTO()
                {
                    Name = employee.Name,
                    Surname = employee.Surname,
                    Phone = employee.Phone,
                    CompanyId = employee.CompanyId,
                    Passport = new PassportDTO(employee.Passport.Type, employee.Passport.Number),
                    Department = new DepartmentDTO(employee.Department.Name, employee.Department.Phone)
                };

                employeesResult.Add(employeeLocal);
            }
            return employeesResult;
        }


    }
}
