using SmartWay.Postgres.Models;

namespace SmartWay.Helpers
{
    internal class StaffService
    {
        // Парсинг/Проверка на string.Empty у employee
        public static void ParseEmployee(Employee employee)
        {
            employee.Name = employee.Name == string.Empty ? null : employee.Name;
            employee.Surname = employee.Surname == string.Empty ? null : employee.Surname;
            employee.Phone = employee.Phone == string.Empty ? null : employee.Phone;

            employee.Passport.Type = employee.Passport.Type == string.Empty ? null : employee.Passport.Type;
            employee.Passport.Number = employee.Passport.Number == string.Empty ? null : employee.Passport.Number;

            employee.Department.Name = employee.Department.Name == string.Empty ? null : employee.Department.Name;
            employee.Department.Phone = employee.Department.Phone == string.Empty ? null : employee.Department.Phone;
        }

    }
}
