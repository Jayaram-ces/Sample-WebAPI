namespace SampleAPI.Models
{
    public class GetEmployeeParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } 
        public string? Search { get;set; }
        public string? FilterByRole { get; set;}
        public bool IncludeInActive { get; set;}
    }
}
