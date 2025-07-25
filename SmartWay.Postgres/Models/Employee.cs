namespace SmartWay.Postgres.Models
{
    public class Employee
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int CompanyId { get; set; } = -1;

        public Passport Passport { get; set; }
        public Department Department { get; set; }

    }
}
