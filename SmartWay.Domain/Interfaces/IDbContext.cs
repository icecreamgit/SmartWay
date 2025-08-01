using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWay.Postgres.Models;

namespace SmartWay.Postgres.Interfaces
{
    public interface IDbContext
    {
        Task<int?> AddEmployee(Employee employee);
        Task DeleteEmployee(int Id);
        Task<List<Employee>> GetAllEmployeeFromCompany(int companyId);
        Task<List<Employee>> GetAllEmployeeFromDepartment(string departmentName);
        Task UpdateEmployeeInfo(int Id, Employee employee);
    }
}
