using System.Data.Entity;

namespace LinqExpressionProjection.Test.Model
{
    class ProjectsDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Subproject> Subprojects { get; set; }
    }
}
