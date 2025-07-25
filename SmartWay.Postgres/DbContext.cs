using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.TypeMapping;
using SmartWay.Postgres.Interfaces;
using SmartWay.Postgres.Models;
using System.ComponentModel.Design;
using System.Data;
using System.Transactions;

namespace SmartWay.Postgres
{
    public class DbContext : IDbContext
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILoggerEasy _logger;

        public DbContext(string connectionString, ILoggerEasy logger) 
        {
            _dbConnection = new NpgsqlConnection(connectionString);
            _logger = logger;
        }

        public async Task<int?> AddEmployee(Employee employee)
        {
            _dbConnection.Open();
            using var transsaction = _dbConnection.BeginTransaction();

            try
            {
                // Добавление Работника
                var queryEmployee = @"insert into employees (name, surname, phone, companyid)
                                         values (@name, @surname, @phone, @companyId)
                                         returning id";

                var employeeObj = await _dbConnection.ExecuteScalarAsync(queryEmployee, employee, transsaction);

                if (employeeObj is null)
                {
                    throw new Exception("Can't AddEmployee");
                }
                var employeeId = (int)employeeObj;

                // Добавление департамента
                var queryDepartment = @"insert into departments (name, phone, employeeid)
                                           values (@name, @phone, @employeeId)";

                await _dbConnection.ExecuteAsync(queryDepartment, new 
                {
                    employee.Department.Name,
                    employee.Department.Phone,
                    employeeId = employeeId
                }, transsaction);



                // Добавление пасспорта
                var queryPassport = @"insert into passports (type, number, employeeid) 
                                         values (@type, @number, @employeeId)";

                await _dbConnection.ExecuteAsync(queryPassport, new 
                    {
                        employee.Passport.Type,
                        employee.Passport.Number,
                        employeeId = employeeId,
                    }, transsaction);

                transsaction.Commit();
                return employeeId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                transsaction.Rollback();
                return null;
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task DeleteEmployee(int Id)
        {
            _dbConnection.Open();
            using var transsaction = _dbConnection.BeginTransaction();

            // Реализовано "жёсткое удаление"
            try
            {
                var query = @"delete from employees where id = @id";
                await _dbConnection.ExecuteAsync(query, new
                {
                    Id
                }, transsaction);

                transsaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                transsaction.Rollback();
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task<List<Employee>> GetAllEmployeeFromCompany(int companyId)
        {
            _dbConnection.Open();
            
            try
            {
                List<Employee> employees = new List<Employee>();

                var query = @"select e.id as id, e.name as Name, e.surname as Surname, e.phone as Phone, e.companyid as CompanyId,
                                    p.id as passport_id, p.type as Type, p.number as Number,
                                    d.id as department_id, d.name as Name, d.phone as Phone
                                from employees e 
                                join passports as p on e.id = p.employeeid
                                join departments d on e.id = d.employeeid
                                where e.companyid = @companyId";

                var employeesList = (await _dbConnection.QueryAsync<Employee, Passport, Department, Employee>
                    (query,
                    (employee, passport, department) =>
                        {
                            employee.Passport = passport;
                            employee.Department = department;
                            return employee;
                        },
                    new { companyId },
                    splitOn: "passport_id, department_id")
                    ).ToList();

                return employeesList;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                return null;
            }
            finally
            {
                _dbConnection.Close();
            }

        }

        public async Task<List<Employee>> GetAllEmployeeFromDepartment(string departmentName)
        {
            _dbConnection.Open();

            try
            {
                var query = @"select e.id as id, e.name as Name, e.surname as Surname, e.phone as Phone, e.companyid as CompanyId,
                                    p.id as passport_id, p.type as Type, p.number as Number,
                                    d.id as department_id, d.name as Name, d.phone as Phone
                                from employees e 
                                join passports as p on e.id = p.employeeid
                                join departments d on e.id = d.employeeid
                                where d.name = @departmentName";

                var employeeList = (await _dbConnection.QueryAsync<Employee, Passport, Department, Employee>
                    (query,
                    (employee, passport, department) =>
                        {
                            employee.Passport = passport;
                            employee.Department = department;
                            return employee;
                        },
                    new { departmentName },
                    splitOn: "passport_id, department_id"
                    )
                    ).ToList();
                return employeeList;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                return null;
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task UpdateEmployeeInfo(int Id, Employee employee)
        {
            _dbConnection.Open();
            using var transsaction = _dbConnection.BeginTransaction();

            try
            {
                var queryFindEmployee = @"select * from employees e
                                            where id = @Id";

                var employeeFinded = (await _dbConnection.QueryAsync<Employee>(queryFindEmployee,
                    new { Id })).FirstOrDefault();

                if (employeeFinded is null)
                {
                    throw new Exception("UpdateEmployeeInfo: Have Got Invalid Id");
                }

                // Изменение полей employee
                var employeeSetFields = @"update employees set 
	                                            name = COALESCE(@name, name),
	                                            surname = COALESCE(@surname, surname),
	                                            phone = COALESCE(@phone, phone),
	                                            companyid = COALESCE(@companyid, companyid)
                                            where id = @id";

                await _dbConnection.QueryAsync(employeeSetFields, new 
                {
                    employee.Name,
                    employee.Surname,
                    employee.Phone,
                    employee.CompanyId,
                    id = Id
                }, transsaction);

                // Изменение полей Passport
                var passportSetFields = @"update passports set 
	                                            type = COALESCE(@type, type),
	                                            number = COALESCE(@number, number)
                                            where employeeid = @employeeid";
                await _dbConnection.ExecuteAsync(passportSetFields, new 
                {
                    employee.Passport.Type,
                    employee.Passport.Number,
                    employeeid = Id
                }, transsaction);


                // Изменение полей Department
                var departmentSetFields = @"update departments set 
	                                            name = COALESCE(@name, name),
	                                            phone = COALESCE(@phone, phone)
                                            where employeeid = @employeeid";
                
                await _dbConnection.ExecuteAsync(departmentSetFields, new
                {
                    employee.Department.Name,
                    employee.Department.Phone,
                    employeeid = Id
                }, transsaction);

                transsaction.Commit();
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                transsaction.Rollback();
            }
            finally
            {
                _dbConnection.Close();
            }
        }
    }
}
