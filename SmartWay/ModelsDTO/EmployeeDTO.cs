namespace SmartWay.ModelsDTO
{
    public class EmployeeDTO()
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int CompanyId { get; set; } = -1;

        public PassportDTO Passport { get; set; }
        public DepartmentDTO Department { get; set; }

    }
}
