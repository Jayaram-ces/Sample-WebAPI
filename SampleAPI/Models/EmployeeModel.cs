using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
