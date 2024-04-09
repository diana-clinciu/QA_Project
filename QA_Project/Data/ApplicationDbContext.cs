using Microsoft.EntityFrameworkCore;
using QA_Project.Models;

namespace QA_Project.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Pet> Pets { get; set; }
    }
}
