namespace SmartWay.Postgres.Models
{
    public class Passport
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}
