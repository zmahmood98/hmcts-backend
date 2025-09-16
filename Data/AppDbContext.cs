using Microsoft.EntityFrameworkCore;
using HmctsBackend.Models;

namespace HmctsBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<TaskItem> Tasks { get; set; }
    }
}