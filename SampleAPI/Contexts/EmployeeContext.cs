using Microsoft.EntityFrameworkCore;
using SampleAPI.Models;

namespace SampleAPI.Contexts
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options) { }

        public DbSet<EmployeeModel> Employees { get; set; }
    }
}
