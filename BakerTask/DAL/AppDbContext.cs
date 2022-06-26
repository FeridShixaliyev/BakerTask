using BakerTask.Models;
using Microsoft.EntityFrameworkCore;

namespace BakerTask.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions options):base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
