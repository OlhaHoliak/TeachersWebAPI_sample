using Microsoft.EntityFrameworkCore;

namespace TeacherApi.Models
{
    public class TeachersContext : DbContext
    {
        public TeachersContext(DbContextOptions<TeachersContext> options) 
            : base(options)
        {
        }

        public DbSet<Teacher> Teachers { get; set; }
    }
}
