namespace SmartWay.Postgres.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}
